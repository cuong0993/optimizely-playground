using AlloyDemo.Models.Pages;
using EPiServer.Find.UnifiedSearch;

namespace AlloyDemo.Models.ViewModels
{
    public class FindSearchPageViewModel : PageViewModel<SearchPage>
    {
        public FindSearchPageViewModel(SearchPage currentPage,
            string searchQuery) : base(currentPage)
        {
            SearchQuery = searchQuery;
        }

        public string SearchQuery { get; }

        public UnifiedSearchResults Results { get; set; }
    }
}