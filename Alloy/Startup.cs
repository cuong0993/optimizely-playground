using Alloy.Extensions;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Configurations;
using EPiServer.Cms.TinyMce;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.Scheduler;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http.Features;

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
        services.Configure<UploadOptions>(x =>
        {
            x.FileSizeLimit = 1073741824;
        });
        services.Configure<FormOptions>(x =>
        {
            x.MultipartBodyLengthLimit = long.MaxValue;
        });
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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
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
        });
        app.Use(async (httpContext, next) =>
        {
            if (!httpContext.User.Identity.IsAuthenticated &&
                httpContext.Request.Path.StartsWithSegments("/Util/Logout"))
            {
                httpContext.Response.Redirect("/");
                return;
            }
            await next();
        });
    }
}
