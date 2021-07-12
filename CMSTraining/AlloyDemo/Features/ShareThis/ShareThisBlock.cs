using AlloyDemo.Models;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using System.ComponentModel.DataAnnotations;

namespace AlloyDemo.Features.ShareThis
{
    [SiteContentType(DisplayName = "Share This", 
        Description = "Allows a visitor to share a link to a page via Twitter, Facebook, or LinkedIn.")]
    [SiteImageUrl]
    public class ShareThisBlock : BlockData
    {
        [Display(Name = "Display Facebook share",
            GroupName = SystemTabNames.Content, Order = 100)]
        public virtual bool ShareToFacebook { get; set; }

        [Display(Name = "Display Twitter share",
            GroupName = SystemTabNames.Content, Order = 200)]
        public virtual bool ShareToTwitter { get; set; }

        [Display(Name = "Display LinkedIn share",
            GroupName = SystemTabNames.Content, Order = 300)]
        public virtual bool ShareToLinkedIn { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            ShareToFacebook = true;
            ShareToTwitter = true;
            ShareToLinkedIn = true;
        }
    }
}