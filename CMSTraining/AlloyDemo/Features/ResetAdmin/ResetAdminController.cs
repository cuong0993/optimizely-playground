using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using EPiServer.Shell.Security;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AlloyAdvanced.Features.ResetAdmin
{
    public class ResetAdminController : Controller
    {
        // Admin user account
        public const string Username = "Admin";
        public const string Password = "Pa$$w0rd";
        public const string Email = "admin@alloy.com";
        public const string Role = "WebAdmins";

        private readonly IContentSecurityRepository securityRepository;
        private readonly UIUserProvider users;
        private readonly UIRoleProvider roles;

        public ResetAdminController(IContentSecurityRepository securityRepository,
            IPageCriteriaQueryService pageFinder,
            UIUserProvider users, UIRoleProvider roles)
        {
            this.securityRepository = securityRepository;
            this.users = users;
            this.roles = roles;
        }

        //
        // GET: /ResetAdmin
        public ActionResult Index()
        {
            return View("~/Features/ResetAdmin/ResetAdmin.cshtml");
        }

        //
        // POST: /ResetAdmin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(string submit)
        {
            // Use EPiServer classes to create roles and users

            UIUserCreateStatus status;
            IEnumerable<string> errors = new List<string>();

            if (!roles.RoleExists(Role))
            {
                roles.CreateRole(Role);
            }
            
            users.DeleteUser(Username, true);

            var newUser = users.CreateUser(Username, Password, Email,
                passwordQuestion: null, passwordAnswer: null,
                isApproved: true,
                status: out status, errors: out errors);

            if (status == UIUserCreateStatus.Success)
            {
                roles.AddUserToRoles(Username, new[] { Role });
            }

            // Use EPiServer classes to give access rights to Root

            SetSecurity(ContentReference.RootPage, "CmsAdmins", AccessLevel.FullAccess);
            SetSecurity(ContentReference.RootPage, "WebAdmins", AccessLevel.NoAccess);
            SetSecurity(ContentReference.RootPage, "Administrators", AccessLevel.NoAccess);

            ResetAdmin.IsEnabled = false;

            ViewData["message"] = $"Reset Admin completed successfully.";

            return View("~/Features/ResetAdmin/ResetAdmin.cshtml");
        }

        private void SetSecurity(ContentReference reference, string role, AccessLevel level, bool overrideInherited = false)
        {
            IContentSecurityDescriptor permissions = securityRepository.Get(reference).CreateWritableClone() as IContentSecurityDescriptor;
            if (overrideInherited)
            {
                if (permissions.IsInherited) permissions.ToLocal();
            }
            permissions.AddEntry(new AccessControlEntry(role, level));
            securityRepository.Save(reference, permissions, SecuritySaveType.Replace);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!ResetAdmin.IsEnabled)
            {
                filterContext.Result = new HttpNotFoundResult();
                return;
            }
            base.OnAuthorization(filterContext);
        }
    }
}
