using AlloyDemo.Models;
using AlloyDemo.Models.Pages;
using EPiServer.DataAnnotations;

namespace AlloyDemo.Features.UserNotifications
{
    [SiteContentType(DisplayName = "Send Notification",
        GUID = "a05920ff-97fc-4f83-b512-3bf793f84447",
        GroupName = Global.GroupNames.Specialized,
        Description = "Use to send a notification to another Episerver user.")]
    [SiteImageUrl]
    [AvailableContentTypes(IncludeOn = new[] {typeof(StartPage)})]
    public class SendNotificationPage : SitePageData
    {
    }
}