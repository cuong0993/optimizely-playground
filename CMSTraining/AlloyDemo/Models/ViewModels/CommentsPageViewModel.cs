using AlloyDemo.Models.Pages;
using System.Collections.Generic;
using EPiServer.Core;
using AlloyDemo.Models.Blocks;

namespace AlloyDemo.Models.ViewModels
{
    public class CommentsPageViewModel : IPageViewModel<SitePageData>
    {
        public CommentsPageViewModel(SitePageData currentPage)
        {
            this.CurrentPage = currentPage;
        }
        public SitePageData CurrentPage { get; protected set; }

        public IEnumerable<CommentBlock> Comments { get; set; }

        public bool CurrentUserCanAddComments { get; set; }

        public bool ThisPageHasAtLeastOneComment { get; set; }

        public bool StartPageHasCommentsFolder { get; set; }

        public LayoutModel Layout { get; set; }

        public IContent Section { get; set; }
    }
}