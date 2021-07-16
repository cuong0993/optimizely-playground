using System.Text;
using System.Web.Mvc;
using AlloyDemo.Business.Channels;
using AlloyDemo.Models.Pages;
using AlloyDemo.Models.ViewModels;
using EPiServer.Framework.DataAnnotations;

namespace AlloyDemo.Controllers
{
    // the Tag should match the ChannelName of the DisplayChannel
    [TemplateDescriptor(Inherited = true, Tags = new[] {"PDF"})]
    public class ProductPageAsPdfController : PageControllerBase<ProductPage>
    {
        public ActionResult Index(ProductPage currentPage)
        {
            // create HTML to send to PDF
            var sb = new StringBuilder();
            sb.Append($"<h1>{currentPage.Name}</h1>");
            sb.Append($"<h3>{currentPage.MetaDescription}</h3>");
            sb.Append(currentPage.MainBody);

            // generate the PDF
            PDFChannelHelper.GeneratePDF(sb.ToString(), currentPage.Name);

            var model = PageViewModel.Create(currentPage);

            return View(model);
        }
    }
}