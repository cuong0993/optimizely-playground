using AlloyDemo.Controllers;  // PageControllerBase
using AlloyDemo.Models.ViewModels; // PageViewModel
using EPiServer.Notification;     // INotifier, IUserNotificationRepository, QueryableNotificationUserService
using EPiServer.Shell.Security;   // UIUserProvider, IUIUser
using System.Collections.Generic; // Dictionary
using System.Threading.Tasks;     // Task<T>
using System.Web.Mvc;             // ActionResult

namespace AlloyDemo.Features.UserNotifications
{
    public class NotificationsPageController : PageControllerBase<NotificationsPage>
    {
        // some services that we need
        private readonly INotifier notifier;
        private readonly IUserNotificationRepository userNotificationRepository;
        private readonly QueryableNotificationUserService
            queryableNotificationUserService;
        private readonly UIUserProvider userProvider;

        public NotificationsPageController(INotifier notifier,
            IUserNotificationRepository userNotificationRepository,
            QueryableNotificationUserService queryableNotificationUserService,
            UIUserProvider userProvider)
        {
            this.notifier = notifier;
            this.userNotificationRepository = userNotificationRepository;
            this.queryableNotificationUserService = queryableNotificationUserService;
            this.userProvider = userProvider;
        }

        // Notifications API is asynchronous
        // channel parameter can be used to filter messages
        public async Task<ActionResult> Index(
            NotificationsPage currentPage, string channel)
        {
            // always reset the Ignore property to an empty dictionary
            currentPage.Notifications =
                new Dictionary<string, PagedUserNotificationMessageResult>();

            // get a list of the first 30 registered users
            IEnumerable<IUIUser> users = userProvider.GetAllUsers(
                pageIndex: 0, pageSize: 30, totalRecords: out int totalRecords);

            foreach (IUIUser user in users)
            {
                // get an object that represents notifications for each user
                INotificationUser notificationUser = await
                    queryableNotificationUserService.GetAsync(user.Username);

                // build a query that includes read, unread, sent, and unsent messages
                var query = new UserNotificationsQuery
                {
                    Read = null, // include read and unread
                    Sent = null, // include sent and unsent
                    User = notificationUser
                };

                // if a channel name is set, use it to filter the notifications
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    ViewData["channel"] = channel;
                    query.ChannelName = channel;
                }

                // execute the query
                PagedUserNotificationMessageResult result = 
                    await userNotificationRepository
                    .GetUserNotificationsAsync(
                        query, startIndex: 0, maxRows: 20);

                // store the query results for the user
                currentPage.Notifications.Add(user.Username, result);
            }

            return View("~/Features/UserNotifications/NotificationsPage.cshtml",
                PageViewModel.Create(currentPage));
        }
    }
}