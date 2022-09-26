using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Profile;
using AlloyDemo.Models;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Web.Routing;

namespace AlloyDemo.Controllers
{
    /// <summary>
    ///     Used to register a user for first time
    /// </summary>
    public class RegisterController : Controller
    {
        private const string AdminRoleName = "WebAdmins";
        public const string ErrorKey = "CreateError";

        private UIUserProvider UIUserProvider => ServiceLocator.Current.GetInstance<UIUserProvider>();

        private UIRoleProvider UIRoleProvider => ServiceLocator.Current.GetInstance<UIRoleProvider>();

        private UISignInManager UISignInManager => ServiceLocator.Current.GetInstance<UISignInManager>();

        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UIUserCreateStatus status;
                var errors = Enumerable.Empty<string>();
                var result = UIUserProvider.CreateUser(model.Username, model.Password, model.Email, null, null, true,
                    out status, out errors);
                if (status == UIUserCreateStatus.Success)
                {
                    UIRoleProvider.CreateRole(AdminRoleName);
                    UIRoleProvider.AddUserToRoles(result.Username, new[] {AdminRoleName});

                    if (ProfileManager.Enabled)
                    {
                        var profile = EPiServerProfile.Wrap(ProfileBase.Create(result.Username));
                        profile.Email = model.Email;
                        profile.Save();
                    }

                    AdministratorRegistrationPage.IsEnabled = false;
                    SetFullAccessToWebAdmin();
                    var resFromSignIn = UISignInManager.SignIn(UIUserProvider.Name, model.Username, model.Password);
                    if (resFromSignIn) return Redirect(UrlResolver.Current.GetUrl(ContentReference.StartPage));
                }

                AddErrors(errors);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void SetFullAccessToWebAdmin()
        {
            var securityrep = ServiceLocator.Current.GetInstance<IContentSecurityRepository>();
            var permissions =
                securityrep.Get(ContentReference.RootPage).CreateWritableClone() as IContentSecurityDescriptor;
            permissions.AddEntry(new AccessControlEntry(AdminRoleName, AccessLevel.FullAccess));
            securityrep.Save(ContentReference.RootPage, permissions, SecuritySaveType.Replace);
        }

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors) ModelState.AddModelError(ErrorKey, error);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!AdministratorRegistrationPage.IsEnabled)
            {
                filterContext.Result = new HttpNotFoundResult();
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}