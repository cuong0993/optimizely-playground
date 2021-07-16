using System.ComponentModel.DataAnnotations;
using EPiServer.Core;

namespace AlloyDemo.Models.Pages
{
    [SiteContentType(DisplayName = "Shipper",
        AvailableInEditMode = false,
        Description = "A templateless leaf page node to store shipper data.")]
    [SiteImageUrl]
    public class ShipperPage : PageData
    {
        public virtual int ShipperID { get; set; }

        [StringLength(40)] public virtual string CompanyName { get; set; }

        [StringLength(24)] public virtual string Phone { get; set; }

        // properties to enrich the imported data

        public virtual string CostPerUnit { get; set; }
    }
}