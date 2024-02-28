using System.Net;
using System.Net.Mail;
using Alloy.Extensions;
using EPiServer.Cms.Shell;
using EPiServer.Cms.TinyMce;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.Events.Providers.Internal;
using EPiServer.Framework;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;

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

        var client = new SmtpClient();
        var smtpOptions = ServiceLocator.Current.GetService<SmtpOptions>();
        client.Host = smtpOptions.Network.Host;
        client.Port = smtpOptions.Network.Port ?? 587;
        client.Credentials = new NetworkCredential(smtpOptions.Network.UserName, smtpOptions.Network.Password);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = smtpOptions.Network.UseSsl ?? false;
        var msg = new MailMessage();
        msg.From = new MailAddress("onboarding@resend.dev");
        msg.To.Add(new MailAddress("cuong0993@gmail.com"));
        msg.Subject = "Test Email";
        msg.IsBodyHtml = true;
        msg.Body = $"<h1>Test message</h1><p>This is a test message sent on {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} (UTC).</p>";
        Exception exception = null;
        try
        {
            client.Send(msg);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

    }
}
