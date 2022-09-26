using AlloyDemo.Models;
using AlloyDemo.Models.Pages;
using EPiServer.DataAnnotations;

namespace AlloyDemo.Features.ContentApprovals
{
    [SiteContentType(DisplayName = "Content Approvals Manager",
        GUID = "774eeeaa-9ff0-408f-b75e-2fe95cd72761",
        Description = "This page demonstrates how to programmatically manage content approvals.")]
    [SiteImageUrl]
    [AvailableContentTypes(IncludeOn = new[] {typeof(StartPage)})]
    public class ContentApprovalsManagerPage : SitePageData
    {
    }
}