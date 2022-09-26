using System.Collections.Generic;
using AlloyDemo.Models.Pages;
using EPiServer.Core;

namespace AlloyDemo.Models.ViewModels
{
    public class ShippersPageViewModel : IPageViewModel<ShippersPage>
    {
        public ShippersPageViewModel(ShippersPage currentPage)
        {
            CurrentPage = currentPage;
        }

        public IEnumerable<ShipperPage> Shippers { get; set; }
        public ShippersPage CurrentPage { get; set; }
        public LayoutModel Layout { get; set; }
        public IContent Section { get; set; }
    }
}