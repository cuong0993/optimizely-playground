using System.Security.Claims;
using System.Text;
using Alloy.Extensions;
using EPiServer.Cms.Shell;
using EPiServer.Cms.TinyMce;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.Events.Providers.Internal;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

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
            .AddCmsHost().AddCmsHtmlHelpers().AddCmsTagHelpers().AddCmsUI().AddAdmin().AddVisitorGroupsUI().AddTinyMce()
            .AddAlloy()
            .AddEmbeddedLocalization<Startup>();

        // Required by Wangkanai.Detection
        services.AddDetection();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        IdentityModelEventSource.ShowPII = true;
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "azure-cookie";
                options.DefaultChallengeScheme = "azure";
            })
            .AddCookie("azure-cookie", options =>
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
            })
            .AddOpenIdConnect("azure", options =>
            {
                options.SignInScheme = "azure-cookie";
                options.SignOutScheme = "azure-cookie";
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.CallbackPath = "/signin-oidc";
                options.UsePkce = true;

                // If Azure AD is register for multi-tenant
                //options.Authority = "https://login.microsoftonline.com/" + "common" + "/v2.0";
                options.Authority = _configuration["Authentication:Authority"];
                options.ClientId = _configuration["Authentication:ClientId"];
                options.ClientSecret = _configuration["Authentication:ClientSecret"];

                options.Scope.Clear();
                options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                options.Scope.Add(OpenIdConnectScope.Email);
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = "roles",
                    NameClaimType = "name",
                    ValidateIssuer = false
                };

                options.Events.OnRedirectToIdentityProvider = ctx =>
                {
                    // Prevent redirect loop
                    if (ctx.Response.StatusCode == 401) ctx.HandleResponse();

                    return Task.CompletedTask;
                };

                options.Events.OnAuthenticationFailed = context =>
                {
                    context.HandleResponse();
                    context.Response.BodyWriter.WriteAsync(Encoding.ASCII.GetBytes(context.Exception.Message));
                    return Task.CompletedTask;
                };
            });
        
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IClaimsTransformation, ClaimsTransformer>());

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
            endpoints.MapControllerRoute("AspNetCore", "/Login", new { controller = "DefaultPage", action = "Login" });
            endpoints.MapControllerRoute("AspNetCore", "/Logout", new { controller = "DefaultPage", action = "Logout" });
        });
    }
}
