using Alloy.Extensions;
using EPiServer.Authorization;
using EPiServer.Cms.Shell;
using EPiServer.Cms.TinyMce;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.Scheduler;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Geta.NotFoundHandler.Infrastructure.Configuration;
using Geta.NotFoundHandler.Infrastructure.Initialization;
using Geta.NotFoundHandler.Optimizely.Infrastructure.Configuration;

namespace Alloy;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostingEnvironment;

    public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
    {
        _webHostingEnvironment = webHostingEnvironment;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }
        else
        {
            services.AddCmsCloudPlatformSupport(_configuration);
        }

        services
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCmsHost().AddCmsHtmlHelpers().AddCmsTagHelpers().AddCmsUI().AddAdmin().AddVisitorGroupsUI().AddTinyMce()
            .AddAlloy()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>();

        // Required by Wangkanai.Detection
        services.AddDetection();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        services.AddNotFoundHandler(o => o.UseSqlServer(_configuration.GetConnectionString("EPiServerDB")),
            policy => policy.RequireRole(Roles.CmsAdmins));
        services.AddOptimizelyNotFoundHandler();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseNotFoundHandler();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Required by Wangkanai.Detection
        app.UseDetection();
        app.UseSession();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
            endpoints.MapRazorPages();
        });
    }
}
