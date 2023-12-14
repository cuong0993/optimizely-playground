using EPiServer.Cms.WelcomeIntegration.Core.Internal;
using EPiServer.Cms.WelcomeIntegration.UI.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Alloy.Models;

public class ResponsiveDAMImageAssetViewComponent : DAMImageAssetViewComponent
{
    protected override IViewComponentResult InvokeComponent(DAMImageAsset currentContent)
    {
        return new HtmlContentViewComponentResult(new HtmlString($@"
<picture>
  <source media='(min-width:900px)' srcset='https://upload.wikimedia.org/wikipedia/commons/thumb/b/bd/Test.svg/620px-Test.svg.png'>
  <img src='{currentContent.DAMUrl}' title='{currentContent.Name}' class='image-file'>
</picture>
"));
    }
}