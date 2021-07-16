using System.ComponentModel.DataAnnotations;
using AlloyDemo.Business;
using AlloyDemo.Business.EditorDescriptors;
using AlloyDemo.Business.Rendering;
using EPiServer.Web;
using EPiServer.Core;
using AlloyDemo.Business.Selectors;
using EPiServer.Shell.ObjectEditing;

namespace AlloyDemo.Models.Pages
{
    /// <summary>
    /// Represents contact details for a contact person
    /// </summary>
    [SiteContentType(
        GroupName = Global.GroupNames.Specialized)]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-contact.png")]
    public class ContactPage : SitePageData, IContainerPage
    {
        [Display(GroupName = Global.GroupNames.Contact)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference Image { get; set; }

        [Display(GroupName = Global.GroupNames.Contact)]
        public virtual string Phone { get; set; }

        [Display(GroupName = Global.GroupNames.Contact)]
        [UIHint(Global.SiteUIHints.Email)]
        public virtual string Email { get; set; }

        [Display(
            Name = "Region",
            GroupName = Global.GroupNames.Contact,
            Order = 10)]
        [SelectOneEnum(typeof(Region))]
        public virtual Region Region { get; set; }

        [Display(
            Name = "YouTube video",
            GroupName = Global.GroupNames.Contact,
            Order = 20)]
        [SelectOne(SelectionFactoryType = typeof(YouTubeSelectionFactory))]
        [UIHint(Global.SiteUIHints.YouTube)]
        public virtual string YouTubeVideo { get; set; }

        [Display(
            Name = "Home city",
            GroupName = Global.GroupNames.Contact,
            Order = 30)]
        //[SelectOne(SelectionFactoryType = typeof(CitySelectionFactory))]
        [UIHint(Global.SiteUIHints.City)]
        public virtual string HomeCity { get; set; }

        [Display(
            Name = "Other cities",
            GroupName = Global.GroupNames.Contact,
            Order = 40)]
        [SelectMany(SelectionFactoryType = typeof(CitySelectionFactory))]
        [UIHint(Global.SiteUIHints.Cities)]
        public virtual string OtherCities { get; set; }
    }
}
