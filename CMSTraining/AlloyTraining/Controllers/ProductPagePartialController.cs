using AlloyTraining.Models.Pages;
using AlloyTraining.Models.ViewModels;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace AlloyTraining.Controllers
{
    public class ProductPagePartialController 
        : PartialContentController<ProductPage>
    {
        public override ActionResult Index(ProductPage currentPage)
        {
            return PartialView(PageViewModel.Create(currentPage));
        }
    }
}