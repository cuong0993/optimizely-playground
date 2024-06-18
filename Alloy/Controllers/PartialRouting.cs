using Alloy.Models.Pages;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc;

namespace Alloy.Controllers
{
    public class DummyRouteData
    {
    }

    public class DummyController : Controller, IRenderTemplate<DummyRouteData>
    {
        public ActionResult Index()
        {
            return Content("Prutt");
        }
    }

    // Go to https://localhost:5000/en/search/test
    public class DummyRouter : IPartialRouter<SearchPage, DummyRouteData>
    {
        public PartialRouteData GetPartialVirtualPath(DummyRouteData content, UrlGeneratorContext urlGeneratorContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(SearchPage content, UrlResolverContext urlResolverContext)
        {
            var nextSegment = urlResolverContext.GetNextSegment();
            urlResolverContext.RemainingSegments = nextSegment.Remaining;
            return new DummyRouteData();
        }
    }
}
