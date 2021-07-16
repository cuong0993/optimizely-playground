using System.Collections.Generic;
using AlloyDemo.Models.Blocks;
using AlloyDemo.Models.Pages;
using EPiServer.Core;

namespace AlloyDemo.Models.ViewModels
{
    public class CommentsPageViewModel : IPageViewModel<SitePageData>
    {
        public CommentsPageViewModel(SitePageData currentPage)
        {
            CurrentPage = currentPage;
        }

        public IEnumerable<CommentBlock> Comments { get; set; }

        public bool CurrentUserCanAddComments { get; set; }

        public bool ThisPageHasAtLeastOneComment { get; set; }

        public bool StartPageHasCommentsFolder { get; set; }
        public SitePageData CurrentPage { get; protected set; }

        public LayoutModel Layout { get; set; }

        public IContent Section { get; set; }
    }
}