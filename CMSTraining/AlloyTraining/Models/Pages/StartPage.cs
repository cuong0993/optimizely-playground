using System.ComponentModel.DataAnnotations;
using AlloyTraining.Models.Media;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace AlloyTraining.Models.Pages
{
    [ContentType(DisplayName = "Start",
        GUID = "9603f676-acfd-493f-9dce-d15e16ef8855",
        GroupName = SiteGroupNames.Specialized, Order = 10,
        Description = "The home page for a website with an area for blocks and partial pages.")]
    [SiteStartIcon]
    [AvailableContentTypes(Include = new[] {typeof(StandardPage)})]
    public class StartPage : SitePageData
    {
        [CultureSpecific]
        [Display(Name = "Heading", Description =
                "If the Heading is not set, the page falls back to showing the Name.",
            GroupName = SystemTabNames.Content, Order = 10)]
        public virtual string Heading { get; set; }

        [CultureSpecific]
        [Display(Name = "Main body",
            Description =
                "The main body uses the XHTML-editor so you can insert, for example text, images, and tables.",
            GroupName = SystemTabNames.Content, Order = 20)]
        public virtual XhtmlString MainBody { get; set; }

        [CultureSpecific]
        [Display(Name = "Main content area",
            Description = "Drag and drop images, blocks, folders, and pages with partial templates.",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        [AllowedTypes(typeof(StandardPage), typeof(BlockData),
            typeof(ImageData), typeof(ContentFolder), typeof(PdfFile))]
        public virtual ContentArea MainContentArea { get; set; }
    }
}