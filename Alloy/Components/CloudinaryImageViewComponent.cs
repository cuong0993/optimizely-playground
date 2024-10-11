using Alloy.Models.Media;
using Alloy.Models.ViewModels;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using OptimizelyPublicUrlIssue.BLL.CmsUnits;

namespace Alloy.Models.ViewModels;


public class CloudinaryImageViewComponent: PartialContentComponent<OptimizelyPublicUrlIssueAssetImageData>
{
    private readonly UrlResolver _urlResolver;

    public CloudinaryImageViewComponent(UrlResolver urlResolver)
    {
        _urlResolver = urlResolver;
    }

    /// <summary>
    /// The index action for the image file. Creates the view model and renders the view.
    /// </summary>
    /// <param name="currentContent">The current image file.</param>
    protected override IViewComponentResult InvokeComponent(OptimizelyPublicUrlIssueAssetImageData currentContent)
    {
        var model = new CloudinaryImageViewModel
        {
            Url = currentContent.Url.ToString(),
            Name = currentContent.Name,
            Copyright = "currentContent.Copyright"
        };

        return View(model);
    }
}