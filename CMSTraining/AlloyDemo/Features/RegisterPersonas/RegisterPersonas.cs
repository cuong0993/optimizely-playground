using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using Owin;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AlloyDemo.Features.RegisterPersonas
{
    public static class RegisterPersonas
    {
        private static Func<bool> _isLocalRequest = () => false;

        private static Lazy<bool> _arePersonasRegistered = new Lazy<bool>(() => false);

        private static bool? _isEnabled = null;

        public static bool IsEnabled
        {
            get
            {
                if (_isEnabled.HasValue)
                {
                    return _isEnabled.Value;
                }

                var showPersonasRegistration = _isLocalRequest() && !_arePersonasRegistered.Value;
                if (!showPersonasRegistration)
                {
                    _isEnabled = false;
                }

                return showPersonasRegistration;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public static void UseRegisterPersonas(this IAppBuilder app, Func<bool> isLocalRequest)
        {
            _isLocalRequest = isLocalRequest;
            _arePersonasRegistered = new Lazy<bool>(ArePersonasRegistered);
            GlobalFilters.Filters.Add(new RegisterPersonasActionFilterAttribute());
            if (isLocalRequest())
            {
                AddRoute();
            }
        }

        private static bool ArePersonasRegistered()
        {
            var provider = ServiceLocator.Current.GetInstance<UIUserProvider>();
            foreach (var user in RegisterPersonasController.Users)
            {
                IUIUser u = provider.GetUser(user.UserName);
                if (u == null) return false;
            }
            return true;
        }

        public class RegisterPersonasActionFilterAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var registerUrl = VirtualPathUtility.ToAbsolute("~/RegisterPersonas");
                if (IsEnabled && !context.RequestContext.HttpContext.Request.Path.StartsWith(registerUrl))
                {
                    context.Result = new RedirectResult(registerUrl);
                }
            }
        }

        static void AddRoute()
        {
            var routeData = new RouteValueDictionary();
            routeData.Add("Controller", "RegisterPersonas");
            routeData.Add("action", "Index");
            routeData.Add("id", " UrlParameter.Optional");
            RouteTable.Routes.Add("RegisterPersonas", new Route("{controller}/{action}/{id}", routeData, new MvcRouteHandler()) { RouteExistingFiles = false });
        }
    }
}
