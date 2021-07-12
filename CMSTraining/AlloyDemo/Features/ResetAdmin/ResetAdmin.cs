using Owin;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AlloyAdvanced.Features.ResetAdmin
{
    public static class ResetAdmin
    {
        private static Func<bool> _isLocalRequest = () => false;

        private static Lazy<bool> _isAdminRegistered = new Lazy<bool>(() => false);

        private static bool? _isEnabled = null;

        public static bool IsEnabled
        {
            get
            {
                if (_isEnabled.HasValue)
                {
                    return _isEnabled.Value;
                }

                var showResetAdmin = _isLocalRequest() && !_isAdminRegistered.Value;
                if (!showResetAdmin)
                {
                    _isEnabled = false;
                }

                return showResetAdmin;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public static void UseResetAdmin(this IAppBuilder app, Func<bool> isLocalRequest)
        {
            _isLocalRequest = isLocalRequest;
            _isAdminRegistered = new Lazy<bool>(() => false);
            GlobalFilters.Filters.Add(new ResetAdminActionFilterAttribute());
            if (isLocalRequest())
            {
                AddRoute();
            }
        }

        public class ResetAdminActionFilterAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var registerUrl = VirtualPathUtility.ToAbsolute("~/ResetAdmin");
                if (IsEnabled && !context.RequestContext.HttpContext.Request.Path.StartsWith(registerUrl))
                {
                    context.Result = new RedirectResult(registerUrl);
                }
            }
        }

        static void AddRoute()
        {
            var routeData = new RouteValueDictionary();
            routeData.Add("Controller", "ResetAdmin");
            routeData.Add("action", "Index");
            routeData.Add("id", " UrlParameter.Optional");
            RouteTable.Routes.Add("ResetAdmin", new Route("{controller}/{action}/{id}", routeData, new MvcRouteHandler()) { RouteExistingFiles = false });
        }
    }
}
