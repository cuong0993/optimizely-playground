using System.Collections.Generic;
using AlloyDemo.Models;
using AlloyDemo.Models.Pages;
using EPiServer.DataAnnotations;
using EPiServer.Notification;

// [SiteContentType], [SiteImageUrl]
// SitePageData, StartPage
// [AvailableContentTypes], [Ignore]
// PagedUserNotificationMessageResult

// Dictionary<T>

namespace AlloyDemo.Features.UserNotifications
{
    [SiteContentType(DisplayName = "Notifications",
        GroupName = Global.GroupNames.Specialized,
        GUID = "9d8ce416-9056-4651-b727-fcb30773bead",
        Description = "Use to manage user notifications.")]
    [SiteImageUrl]
    [AvailableContentTypes(IncludeOn = new[] {typeof(StartPage)})]
    public class NotificationsPage : SitePageData
    {
        [Ignore] // this property is not stored in the database
        public virtual Dictionary<string,
            PagedUserNotificationMessageResult> Notifications { get; set; }
    }
}