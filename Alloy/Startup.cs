using System.Security.Claims;
using Alloy.Extensions;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Configurations;
using EPiServer.Cms.TinyMce;
using EPiServer.Cms.TinyMce.Core;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.Shell.Modules;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
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
         services.Configure<TinyMceConfiguration>(config =>
            {




                config.Default()
                    .AddPlugin("media wordcount anchor code searchreplace")
                    .Toolbar("help epi-personalized-content blocks fontfamily fontsize | epi-personalized-content epi-link anchor numlist bullist indent outdent bold italic underline code",
                        "alignleft aligncenter alignright alignjustify | image epi-image-editor media | epi-dnd-processor | forecolor backcolor | removeformat | searchreplace fullscreen")
                    .AddSetting("image_caption", true)
                    .AddSetting("image_advtab", true)
                    .AddSetting("resize", "both")
                    .AddSetting("height", 400);

                config.Default()
                    .AddEpiserverSupport()
                    //.AddExternalPlugin("icons", "/ClientResources/Scripts/fontawesomeicons.js")
                    .AddSetting("extended_valid_elements", "i[class], span");
                    //.ContentCss(new[] { "/ClientResources/Styles/fontawesome.min.css",
                    //    "https://fonts.googleapis.com/css?family=Roboto:100,100i,300,300i,400,400i,500,500i,700,700i,900,900i",
                    //    "/ClientResources/Styles/TinyMCE.css" });
            });
        
        //services.Configure<ProtectedModuleOptions>(p => p.RootPath = "~/TheNewUiPath");
       //services.Configure<UIOptions>(p => p.EditUrl = new Uri("~/TheNewUiPath/CMS", UriKind.Relative));

       // services.Configure<UploadOptions>(x => { x.FileSizeLimit = 1073741824; });
       // services.Configure<FormOptions>(x => { x.MultipartBodyLengthLimit = long.MaxValue; });
        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory",
                Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }
        else
        {
            services.AddCmsCloudPlatformSupport(_configuration);
        }

        services
            .AddCmsAspNetIdentity<ApplicationUser>();
        services.AddCmsHost().AddCmsHtmlHelpers().AddCmsTagHelpers().AddCmsUI().AddAdmin().AddVisitorGroupsUI()
            .AddTinyMce()
            .AddAlloy()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>().AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Events.OnSignedIn = async ctx =>
                {
                    if (ctx.Principal?.Identity is ClaimsIdentity claimsIdentity)
                    {
                        // Syncs user and roles so they are available to the CMS
                        var synchronizingUserService = ctx
                            .HttpContext
                            .RequestServices
                            .GetRequiredService<ISynchronizingUserService>();

                        await synchronizingUserService.SynchronizeAsync(claimsIdentity);
                    }
                };
            });


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
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        // Required by Wangkanai.Detection
        app.UseDetection();
        app.UseSession();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapContent(); });
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