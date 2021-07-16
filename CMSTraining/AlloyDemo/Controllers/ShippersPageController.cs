using System.Web.Mvc;
using AlloyDemo.Models.Pages;
using AlloyDemo.Models.ViewModels;
using EPiServer;

namespace AlloyDemo.Controllers
{
    public class ShippersPageController : PageControllerBase<ShippersPage>
    {
        private readonly IContentLoader loader;

        public ShippersPageController(IContentLoader loader)
        {
            this.loader = loader;
        }

        public ActionResult Index(ShippersPage currentPage)
        {
            var model = new ShippersPageViewModel(currentPage);
            model.Shippers = loader.GetChildren<ShipperPage>(currentPage.ContentLink);
            return View(model);
        }
    }
}