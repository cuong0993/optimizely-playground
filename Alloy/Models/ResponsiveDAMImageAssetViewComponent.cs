using System.Diagnostics.CodeAnalysis;
using EPiServer.Cms.WelcomeIntegration.Core;
using EPiServer.Cms.WelcomeIntegration.Core.Internal;
using EPiServer.Cms.WelcomeIntegration.UI;
using EPiServer.Cms.WelcomeIntegration.UI.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Alloy.Models;

public class ResponsiveDAMImageAssetViewComponent : DAMImageAssetViewComponent
{
    public ResponsiveDAMImageAssetViewComponent([NotNull] IDAMAssetMetadataService metadataService,
        [NotNull] IDAMAssetIdentityResolver dAmAssetIdentityResolver) : base(metadataService, dAmAssetIdentityResolver)
    {
    }

    protected override IViewComponentResult InvokeComponent(DAMImageAsset currentContent)
    {
        var imageHtml =
            (base.InvokeComponent(currentContent) as HtmlContentViewComponentResult).EncodedContent.ToString();
        return new HtmlContentViewComponentResult(new HtmlString($@"
<picture>
  <source media='(min-width:900px)' srcset='https://upload.wikimedia.org/wikipedia/commons/thumb/b/bd/Test.svg/620px-Test.svg.png'>
  {imageHtml}
</picture>
"));
    }
}