using AlloyDemo.Controllers;
using AlloyDemo.Models.ViewModels;
using EPiServer.Notification;
using EPiServer.Shell.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AlloyDemo.Features.UserNotifications
{
    public class SendNotificationPageController : PageControllerBase<SendNotificationPage>
    {
        // some services that we need
        private readonly INotifier notifier;
        private readonly IUserNotificationRepository userNotificationRepository;
        private readonly QueryableNotificationUserService queryableNotificationUserService;
        private readonly UIUserProvider userProvider;

        public SendNotificationPageController(INotifier notifier,
            IUserNotificationRepository userNotificationRepository,
            QueryableNotificationUserService queryableNotificationUserService,
            UIUserProvider userProvider)
        {
            this.notifier = notifier;
            this.userNotificationRepository = userNotificationRepository;
            this.queryableNotificationUserService = queryableNotificationUserService;
            this.userProvider = userProvider;
        }

        public ActionResult Index(SendNotificationPage currentPage)
        {
            // get a list of the first 30 registered users
            IEnumerable<IUIUser> users = userProvider.GetAllUsers(
                pageIndex: 0, pageSize: 30, totalRecords: out int totalRecords);

            // store the user names in ViewData so they can be used in a dropdown listbox
            ViewData["users"] = users.OrderBy(user => user.Username)
                .Select(user => user.Username).ToArray();

            return View("~/Features/UserNotifications/SendNotificationPage.cshtml",
                PageViewModel.Create(currentPage));
        }

        [HttpPost]
        // Notifications API is asynchronous
        // parameters are for fields in the posted form
        public async Task<ActionResult> Index(SendNotificationPage currentPage,
            string from, string to, string subject, string content, string uri,
            string when, DateTime? whenDateTime, int? whenDelay)
        {
            IEnumerable<IUIUser> users = userProvider.GetAllUsers(
                pageIndex: 0, pageSize: 30, totalRecords: out int totalRecords);

            INotificationUser sender = await
                queryableNotificationUserService.GetAsync(from);

            var receivers = new List<INotificationUser>();
            string[] usernames = to.Split(';');
            foreach (var username in usernames)
            {
                INotificationUser user = await
                    queryableNotificationUserService.GetAsync(username);
                receivers.Add(user);
            }

            NotificationMessage message;

            if (whenDelay.HasValue)
            {
                message = new DelayedNotificationMessage();
                (message as DelayedNotificationMessage).Delay = TimeSpan.FromMinutes(whenDelay.Value);
            }
            else if (whenDateTime.HasValue)
            {
                message = new ScheduledNotificationMessage();
                (message as ScheduledNotificationMessage).SendAt = whenDateTime.Value;
            }
            else
            {
                message = new NotificationMessage();
            }

            if (!string.IsNullOrWhiteSpace(uri)) message.Category = new Uri(uri);
            message.ChannelName = Global.NotificationChannel;
            message.Content = content;
            message.Subject = subject;
            message.Recipients = receivers;
            message.Sender = sender;
            message.TypeName = Global.NotificationChannel;

            await notifier.PostNotificationAsync(message);

            ViewData["users"] = users.OrderBy(user => user.Username)
                .Select(user => user.Username).ToArray();
            ViewData["messageSent"] = "Your message was sent successfully.";

            return View("~/Features/UserNotifications/SendNotificationPage.cshtml",
                PageViewModel.Create(currentPage));
        }
    }
}