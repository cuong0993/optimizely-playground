using AlloyDemo.Models.Pages;
using EPiServer.Core;
using System.Collections.Generic;

namespace AlloyDemo.Models.ViewModels
{
    public class ShippersPageViewModel : IPageViewModel<ShippersPage>
    {
        public ShippersPageViewModel(ShippersPage currentPage)
        {
            CurrentPage = currentPage;
        }
        public ShippersPage CurrentPage { get; set; }
        public LayoutModel Layout { get; set; }
        public IContent Section { get; set; }
        public IEnumerable<ShipperPage> Shippers { get; set; }
    }
}