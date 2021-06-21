using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace AlloyTraining.Models.Pages
{
    [ContentType(DisplayName = "Start",
    GUID = "9603f676-acfd-493f-9dce-d15e16ef8855",
    GroupName = SiteGroupNames.Specialized, Order = 10,
    Description = "The home page for a website with an area for blocks and partial pages.")]
    public class StartPage : PageData
    {
        [CultureSpecific]
        [Display(Name = "Heading", Description =
        "If the Heading is not set, the page falls back to showing the Name.",
        GroupName = SystemTabNames.Content, Order = 10)]
        public virtual string Heading { get; set; }
        [CultureSpecific]
        [Display(Name = "Main body",
        Description = "The main body uses the XHTML-editor so you can insert, for example text, images, and tables.",
        GroupName = SystemTabNames.Content, Order = 20)]
        public virtual XhtmlString MainBody { get; set; }
        [Display(Name = "Main content area",
        Description = "The main content area contains an ordered collection to content references, for example blocks, media assets, and pages.",
        GroupName = SystemTabNames.Content, Order = 30)]
        public virtual ContentArea MainContentArea { get; set; }
    }
}