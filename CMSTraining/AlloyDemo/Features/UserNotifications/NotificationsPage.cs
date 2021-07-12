using AlloyDemo.Models;       // [SiteContentType], [SiteImageUrl]
using AlloyDemo.Models.Pages; // SitePageData, StartPage
using EPiServer.DataAnnotations;  // [AvailableContentTypes], [Ignore]
using EPiServer.Notification;     // PagedUserNotificationMessageResult
using System.Collections.Generic; // Dictionary<T>

namespace AlloyDemo.Features.UserNotifications
{
    [SiteContentType(DisplayName = "Notifications",
        GroupName = Global.GroupNames.Specialized,
        GUID = "9d8ce416-9056-4651-b727-fcb30773bead", 
        Description = "Use to manage user notifications.")]
    [SiteImageUrl]
    [AvailableContentTypes(IncludeOn = new[] { typeof(StartPage) })]
    public class NotificationsPage : SitePageData
    {
        [Ignore] // this property is not stored in the database
        public virtual Dictionary<string,
            PagedUserNotificationMessageResult> Notifications
        { get; set; }
    }
}