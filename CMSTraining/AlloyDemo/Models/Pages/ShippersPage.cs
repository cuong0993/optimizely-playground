using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace AlloyDemo.Models.Pages
{
    [SiteContentType(DisplayName = "Shippers",
        Description = "Displays a list of imported shippers.")]
    [SiteImageUrl]
    [AvailableContentTypes(
        Availability = Availability.Specific,
        Include = new[] {typeof(ShipperPage)},
        IncludeOn = new[] {typeof(StartPage)})]
    public class ShippersPage : SitePageData
    {
        public virtual int DefaultShipper { get; set; }
    }
}