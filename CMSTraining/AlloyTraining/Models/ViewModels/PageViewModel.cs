using System.Collections.Generic;
using AlloyTraining.Models.Pages;
using EPiServer.Core;

namespace AlloyTraining.Models.ViewModels
{
    public class PageViewModel<T> 
        : IPageViewModel<T> where T : SitePageData
    {
        public T CurrentPage { get; set; }
        public StartPage StartPage { get; set; }
        public IEnumerable<SitePageData> MenuPages { get; set; }
        public IContent Section { get; set; }

        // constructors cannot infer the generic type so you must:
        // var model = new PageViewModel<ProductPage>(currentPage);
        public PageViewModel(T currentPage)
        {
            CurrentPage = currentPage;
        }
    }

    public static class PageViewModel
    {
        // methods can infer the generic type so you can:
        // var model = PageViewModel.Create(currentPage);
        public static PageViewModel<T> Create<T>(T currentPage)
            where T : SitePageData
        {
            return new PageViewModel<T>(currentPage);
        }
    }
}