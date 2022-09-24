using Alloy.Models.Pages;
using EPiServer.Web.Routing;

namespace Alloy.Models.ViewModels;

public class SearchContentModel : PageViewModel<SearchPage>
{
    public SearchContentModel(SearchPage currentPage)
        : base(currentPage)
    {
        string internalUrl = currentPage.Url.ToString();
        String resolvedUrl = UrlResolver.Current.GetUrl(internalUrl);
        int d = 0;

    }

    public bool SearchServiceDisabled { get; set; }

    public string SearchedQuery { get; set; }

    public int NumberOfHits { get; set; }

    public IEnumerable<SearchHit> Hits { get; set; }

    public class SearchHit
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Excerpt { get; set; }
    }
}
