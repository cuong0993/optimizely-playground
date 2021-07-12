using System;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Framework.DataAnnotations;
using AlloyDemo.Models.Pages;
using AlloyDemo.Models.ViewModels;
using AlloyDemo.Models.Blocks;
using EPiServer.Core;
using AlloyDemo.Models;
using System.Collections.Generic;
using EPiServer.Filters;
using EPiServer.Framework.Web;

namespace AlloyDemo.Controllers
{
    /// <summary>
    /// Concrete controller that handles all page types that don't have their own specific controllers.
    /// </summary>
    /// <remarks>
    /// Note that as the view file name is hard coded it won't work with DisplayModes (ie Index.mobile.cshtml).
    /// For page types requiring such views add specific controllers for them. Alterntively the Index action 
    /// could be modified to set ControllerContext.RouteData.Values["controller"] to type name of the currentPage
    /// argument. That may however have side effects.
    /// </remarks>
    [TemplateDescriptor(Inherited = true)]
    public class DefaultPageController : PageControllerBase<SitePageData>
    {
        public ViewResult Index(SitePageData currentPage, string orderBy = "ASC")
        {
            IPageViewModel<SitePageData> model = CreateModel(currentPage);

            if (currentPage is IContentWithComments)
            {
                var commentsModel = new CommentsPageViewModel(currentPage);

                // get (or create if necessary) the start page's comments folder
                var start = repo.Get<StartPage>(ContentReference.StartPage);

                ContentReference siteCommentsFolderReference;
                if (!ContentReference.IsNullOrEmpty(start.CommentFolder))
                {
                    siteCommentsFolderReference = start.CommentFolder;
                }
                else
                {
                    // create the site comments folder inside the "For This Site" block folder
                    ContentFolder siteCommentsFolder = repo.GetDefault<ContentFolder>(ContentReference.SiteBlockFolder);
                    siteCommentsFolder.Name = "Comments";
                    siteCommentsFolderReference = repo.Save(siteCommentsFolder,
                        EPiServer.DataAccess.SaveAction.Publish,
                        EPiServer.Security.AccessLevel.NoAccess);

                    // set reference to the site comments folder on the start page
                    start = start.CreateWritableClone() as StartPage;
                    start.CommentFolder = siteCommentsFolderReference;
                    repo.Save(start,
                        EPiServer.DataAccess.SaveAction.Publish,
                        EPiServer.Security.AccessLevel.NoAccess);
                }

                // check if the Start page has its CommentFolder set
                commentsModel.StartPageHasCommentsFolder = (!ContentReference.IsNullOrEmpty(start.CommentFolder));

                bool publishToStartComments = false;

                if (commentsModel.StartPageHasCommentsFolder)
                {
                    // check if the current user has Publish rights to the Start page's CommentFolder
                    ContentFolder startCommentsFolder = repo.Get<ContentFolder>(start.CommentFolder);
                    publishToStartComments = (startCommentsFolder.QueryDistinctAccess(EPiServer.Security.AccessLevel.Publish));
                }

                bool publishToThisPagesComments = false;

                if (!ContentReference.IsNullOrEmpty(currentPage.CommentFolder))
                {
                    // if current page has CommentFolder, get the comments
                    IEnumerable<CommentBlock> blocks = repo.GetChildren<CommentBlock>(currentPage.CommentFolder);

                    // filter comments to only show those: 
                    // 1) with a partial view 
                    // 2) the current user can access
                    // 3) are published
                    var filter = new FilterContentForVisitor(TemplateTypeCategories.MvcPartialView, string.Empty);
                    var listOfBlocks = blocks.OfType<IContent>().ToList();
                    filter.Filter(listOfBlocks);
                    var listOfComments = listOfBlocks.OfType<CommentBlock>();

                    // sort the comments by publish date
                    if (orderBy == "ASC")
                    {
                        commentsModel.Comments = listOfComments.OrderBy(c => c.When);
                    }
                    else
                    {
                        commentsModel.Comments = listOfComments.OrderByDescending(c => c.When);
                    }

                    commentsModel.ThisPageHasAtLeastOneComment = (commentsModel.Comments.Count() > 0);

                    // check if the current user has Publish rights to the current page's CommentFolder
                    ContentFolder commentsFolder = repo.Get<ContentFolder>(currentPage.CommentFolder);
                    publishToThisPagesComments = (commentsFolder.QueryDistinctAccess(EPiServer.Security.AccessLevel.Publish));
                }

                // set flag to indicate the current user can add comments because either:
                // 1) they have Publish rights to Start's CommentFolder
                // 2) they have Publish rights to current page's CommentFolder
                commentsModel.CurrentUserCanAddComments = (publishToStartComments || publishToThisPagesComments);

                model = commentsModel;
            }

            return View(string.Format("~/Views/{0}/Index.cshtml", currentPage.GetOriginalType().Name), model);
        }

        /// <summary>
        /// Creates a PageViewModel where the type parameter is the type of the page.
        /// </summary>
        /// <remarks>
        /// Used to create models of a specific type without the calling method having to know that type.
        /// </remarks>
        private static IPageViewModel<SitePageData> CreateModel(SitePageData page)
        {
            var type = typeof(PageViewModel<>).MakeGenericType(page.GetOriginalType());
            return Activator.CreateInstance(type, page) as IPageViewModel<SitePageData>;
        }

        // use constructor injection to get reference to CMS repository
        private readonly IContentRepository repo;

        public DefaultPageController(IContentRepository repo)
        {
            this.repo = repo;
        }

        [HttpPost]
        public ActionResult AddComment(SitePageData currentPage, string commentName, string commentText)
        {
            // get (or create if necessary) the folder for comments for the current page
            ContentReference commentsFolderReference;
            if (!ContentReference.IsNullOrEmpty(currentPage.CommentFolder))
            {
                commentsFolderReference = currentPage.CommentFolder;
            }
            else
            {
                // get the start page's comments folder
                StartPage start = repo.Get<StartPage>(ContentReference.StartPage);
                ContentReference siteCommentsFolderReference;
                siteCommentsFolderReference = start.CommentFolder;

                // create a comments folder for this page
                ContentFolder commentsFolder = repo.GetDefault<ContentFolder>(start.CommentFolder);
                commentsFolder.Name = $"{currentPage.Name} (Comments)";
                commentsFolderReference = repo.Save(commentsFolder,
                    EPiServer.DataAccess.SaveAction.Publish,
                    EPiServer.Security.AccessLevel.Publish);

                currentPage = currentPage.CreateWritableClone() as SitePageData;
                currentPage.CommentFolder = commentsFolderReference;
                repo.Save(currentPage,
                    EPiServer.DataAccess.SaveAction.Publish,
                    EPiServer.Security.AccessLevel.NoAccess);
            }

            // create the comment in the page's Comments folder
            CommentBlock comment = repo.GetDefault<CommentBlock>(commentsFolderReference);
            comment.When = DateTime.Now;
            commentName = string.IsNullOrWhiteSpace(commentName) ? "Anonymous" : commentName;
            comment.CommentName = commentName;
            comment.CommentText = commentText;

            IContent saveableComment = comment as IContent;
            saveableComment.Name = commentName;
            repo.Save(saveableComment,
                EPiServer.DataAccess.SaveAction.Publish,
                EPiServer.Security.AccessLevel.Publish);

            // reload the original page
            return RedirectToAction("Index");
        }

        public ActionResult ReportComment(SitePageData currentPage, ContentReference id)
        {
            CommentBlock comment = repo.Get<CommentBlock>(id);

            IVersionable version = comment.CreateWritableClone() as IVersionable;

            version.StopPublish = DateTime.Now;

            // to report a comment the current user should have at least Read permission
            repo.Save(version as IContent, 
                EPiServer.DataAccess.SaveAction.Publish, 
                EPiServer.Security.AccessLevel.Read);

            // reload the original page
            return RedirectToAction("Index");
        }
    }
}
