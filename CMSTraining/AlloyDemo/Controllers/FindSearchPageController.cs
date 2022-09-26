using System.Web.Mvc;
using AlloyDemo.Models.Pages;
using AlloyDemo.Models.ViewModels;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Find.Framework.Statistics;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Framework.DataAnnotations;

namespace AlloyDemo.Controllers
{
    [TemplateDescriptor(Default = true)]
    public class FindSearchPageController : PageControllerBase<SearchPage>
    {
        public ActionResult Index(SearchPage currentPage, string q)
        {
            var model = new FindSearchPageViewModel(currentPage, q);

            if (!string.IsNullOrWhiteSpace(q))
            {
                ITypeSearch<ISearchContent> unifiedSearch =
                    SearchClient.Instance.UnifiedSearchFor(q);

                var doNotTrackHeader = Request.Headers.Get("DNT");
                if (doNotTrackHeader == null || doNotTrackHeader.Equals("0")) unifiedSearch = unifiedSearch.Track();

                model.Results = unifiedSearch.ApplyBestBets().GetResult();
            }

            return View(model);
        }
    }
}