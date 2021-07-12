using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace AlloyDemo.Features.ShareThis
{
    public class ShareThisBlockController : BlockController<ShareThisBlock>
    {
        private readonly IPageRouteHelper pageRouteHelper;
        private readonly UrlResolver urlResolver;

        public ShareThisBlockController(IPageRouteHelper pageRouteHelper, UrlResolver urlResolver)
        {
            this.pageRouteHelper = pageRouteHelper;
            this.urlResolver = urlResolver;
        }

        public override ActionResult Index(ShareThisBlock currentBlock)
        {
            var model = new ShareThisBlockViewModel();
            PageData page = pageRouteHelper.Page;
            model.FriendlyUrl = UriSupport.AbsoluteUrlBySettings(
                urlResolver.GetUrl(page.ContentLink));
            model.Settings = currentBlock;
            return PartialView("~/Features/ShareThis/ShareThisBlock.cshtml", model);
        }
    }
}
