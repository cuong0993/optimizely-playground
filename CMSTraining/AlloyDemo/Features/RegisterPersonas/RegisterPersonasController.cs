using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Security;
using EPiServer.Shell.Security;

namespace AlloyDemo.Features.RegisterPersonas
{
    public class RegisterPersonasController : Controller
    {
        // shared password and email domain for all created users
        private const string password = "Pa$$w0rd";
        private const string email = "@alloy.com";

        // stored roles (groups) that are mapped
        private const string accessToAdminView = "AccessToAdminView"; // mapped to CmsAdmins
        private const string accessToEditView = "AccessToEditView"; // mapped to CmsEditors
        private const string personalizersRole = "Personalizers"; // mapped to VisitorGroupAdmins
        private const string developersRole = "Developers"; // mapped to EPiBetaUsers

        // virtual roles that are assigned access rights
        private const string adminsRole = "CmsAdmins"; // full access rights to Root
        private const string editorsRole = "CmsEditors"; // no access rights to Root

        // stored roles that are assigned access rights
        private const string
            contentCreatorsRole = "ContentCreators"; // Read, Create, Edit, Delete access rights to Root

        private const string newsEditorsRole = "NewsEditors"; // full access to News & Events
        private const string marketersRole = "Marketers"; // Create access rights to ProductPage

        private const string
            cLevelExecsRole = "CLevelExecs"; // approve strategic content e.g. Edit access rights for Press Releases

        private const string
            lawyersRole = "Lawyers"; // approve legal content e.g. Edit access rights for Press Releases

        private static readonly string[] rolesToCreate =
        {
            accessToAdminView, accessToEditView, newsEditorsRole, contentCreatorsRole,
            marketersRole, personalizersRole, developersRole, cLevelExecsRole, lawyersRole
        };

        public static UserAndRoles[] Users =
        {
            new UserAndRoles
            {
                UserName = "Alice", // a CMS Admin
                Roles = new[] {accessToAdminView}
            },
            new UserAndRoles
            {
                UserName = "Dana", // a Developer
                Roles = new[] {accessToAdminView, developersRole}
            },
            new UserAndRoles
            {
                UserName = "Eve", // a CMS Editor
                Roles = new[] {accessToEditView, contentCreatorsRole}
            },
            new UserAndRoles
            {
                UserName = "Emily", // a CMS Editor
                Roles = new[] {accessToEditView, contentCreatorsRole}
            },
            new UserAndRoles
            {
                UserName = "Emil", // a CMS Editor
                Roles = new[] {accessToEditView, contentCreatorsRole}
            },
            new UserAndRoles
            {
                UserName = "Nick", // a News Editor
                Roles = new[] {accessToEditView, newsEditorsRole}
            },
            new UserAndRoles
            {
                UserName = "Nancy", // a News Editor
                Roles = new[] {accessToEditView, newsEditorsRole}
            },
            new UserAndRoles
            {
                UserName = "Michelle", // a Marketer
                Roles = new[] {accessToEditView, marketersRole, personalizersRole}
            },
            new UserAndRoles
            {
                UserName = "Carlos", // the CEO
                Roles = new[] {accessToEditView, cLevelExecsRole}
            },
            new UserAndRoles
            {
                UserName = "Catherine", // the CFO
                Roles = new[] {accessToEditView, cLevelExecsRole}
            },
            new UserAndRoles
            {
                UserName = "Larry", // a Lawyer
                Roles = new[] {accessToEditView, lawyersRole}
            },
            new UserAndRoles
            {
                UserName = "Laura", // a Lawyer
                Roles = new[] {accessToEditView, lawyersRole}
            },
            new UserAndRoles
            {
                UserName = "Lori", // a Lawyer
                Roles = new[] {accessToEditView, lawyersRole}
            }
        };

        private readonly IPageCriteriaQueryService pageFinder;
        private readonly UIRoleProvider roles;

        private readonly IContentSecurityRepository securityRepository;
        private readonly UIUserProvider users;

        public RegisterPersonasController(IContentSecurityRepository securityRepository,
            IPageCriteriaQueryService pageFinder,
            UIUserProvider users, UIRoleProvider roles)
        {
            this.securityRepository = securityRepository;
            this.pageFinder = pageFinder;
            this.users = users;
            this.roles = roles;
        }

        //
        // GET: /RegisterPersonas
        public ActionResult Index()
        {
            return View("~/Features/RegisterPersonas/RegisterPersonas.cshtml");
        }

        //
        // POST: /RegisterPersonas
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(string submit)
        {
            var countOfRolesCreated = 0;
            var countOfUsersCreated = 0;

            #region Use EPiServer classes to create roles and users

            UIUserCreateStatus status;
            IEnumerable<string> errors = new List<string>();

            foreach (var role in rolesToCreate)
                if (!roles.RoleExists(role))
                {
                    roles.CreateRole(role);
                    countOfRolesCreated++;
                }

            foreach (var item in Users)
                if (users.GetUser(item.UserName) == null)
                {
                    var newUser = users.CreateUser(item.UserName, password,
                        $"{item.UserName.ToLower()}{email}",
                        null, null,
                        true,
                        out status, out errors);

                    if (status == UIUserCreateStatus.Success)
                    {
                        countOfUsersCreated++;
                        roles.AddUserToRoles(item.UserName, item.Roles);
                    }
                }

            #endregion

            #region Use EPiServer classes to give access rights to Root, Recycle Bin, and News & Events

            SetSecurity(ContentReference.RootPage, adminsRole, AccessLevel.FullAccess);
            SetSecurity(ContentReference.RootPage, "WebAdmins", AccessLevel.NoAccess);
            SetSecurity(ContentReference.RootPage, "Administrators", AccessLevel.NoAccess);
            SetSecurity(ContentReference.RootPage, contentCreatorsRole,
                AccessLevel.Read | AccessLevel.Create | AccessLevel.Edit | AccessLevel.Delete);
            SetSecurity(ContentReference.RootPage, marketersRole, AccessLevel.Create | AccessLevel.Publish);

            SetSecurity(ContentReference.WasteBasket, adminsRole, AccessLevel.FullAccess);
            SetSecurity(ContentReference.WasteBasket, "Administrators", AccessLevel.NoAccess);

            // find the News & Events page
            var criteria = new PropertyCriteriaCollection
            {
                new PropertyCriteria
                {
                    Name = "PageName",
                    Type = PropertyDataType.LongString,
                    Condition = CompareCondition.Equal,
                    Value = "News & Events"
                }
            };

            var pages = pageFinder.FindPagesWithCriteria(ContentReference.StartPage, criteria);

            if (pages.Count == 1)
            {
                // give News Editors full access and remove all access for others
                var news = pages[0].ContentLink;
                SetSecurity(news, newsEditorsRole, AccessLevel.FullAccess, true);
                SetSecurity(news, contentCreatorsRole, AccessLevel.NoAccess, true);
                SetSecurity(news, marketersRole, AccessLevel.NoAccess, true);
            }

            // find the Press Releases page
            criteria = new PropertyCriteriaCollection
            {
                new PropertyCriteria
                {
                    Name = "PageName",
                    Type = PropertyDataType.LongString,
                    Condition = CompareCondition.Equal,
                    Value = "Press Releases"
                }
            };

            pages = pageFinder.FindPagesWithCriteria(ContentReference.StartPage, criteria);

            if (pages.Count == 1)
            {
                // allow Lawyers and C-Level Execs to edit Press Releases
                // so they can approve/decline changes
                var pressReleases = pages[0].ContentLink;
                SetSecurity(pressReleases, lawyersRole, AccessLevel.Edit, true);
                SetSecurity(pressReleases, cLevelExecsRole, AccessLevel.Edit, true);
            }

            #endregion

            RegisterPersonas.IsEnabled = false;

            ViewData["message"] =
                $"Register personas completed successfully. {countOfRolesCreated} roles created. {countOfUsersCreated} users created and added to roles.";

            return View("~/Features/RegisterPersonas/RegisterPersonas.cshtml");
        }

        private void SetSecurity(ContentReference reference, string role, AccessLevel level,
            bool overrideInherited = false)
        {
            var permissions = securityRepository.Get(reference).CreateWritableClone() as IContentSecurityDescriptor;
            if (overrideInherited)
                if (permissions.IsInherited)
                    permissions.ToLocal();
            permissions.AddEntry(new AccessControlEntry(role, level));
            securityRepository.Save(reference, permissions, SecuritySaveType.Replace);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!RegisterPersonas.IsEnabled)
            {
                filterContext.Result = new HttpNotFoundResult();
                return;
            }

            base.OnAuthorization(filterContext);
        }

        public class UserAndRoles
        {
            public string[] Roles;
            public string UserName;
        }
    }
}