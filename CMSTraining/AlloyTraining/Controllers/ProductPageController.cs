using AlloyTraining.Models.Pages;
using EPiServer;
using System.Web.Mvc;

namespace AlloyTraining.Controllers
{
    public class ProductPageController 
        : PageControllerBase<ProductPage>
    {
        public ProductPageController(IContentLoader loader) : base(loader)
        {
        }

        public ActionResult Index(ProductPage currentPage)
        {
            return View(CreatePageViewModel(currentPage));
        }
    }
}