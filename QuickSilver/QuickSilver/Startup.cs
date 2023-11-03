using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.OpenIDConnect;
using EPiServer.Scheduler;
using EPiServer.ServiceApi;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Anonymous;

namespace QuickSilver
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;

        public Startup(IWebHostEnvironment webHostingEnvironment)
        {
            _webHostingEnvironment = webHostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_webHostingEnvironment.IsDevelopment())
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

                services.Configure<SchedulerOptions>(options => options.Enabled = false);
            }

            services
                .AddCmsAspNetIdentity<ApplicationUser>()
                .AddCommerce()
                .AddAdminUserRegistration()
                .AddEmbeddedLocalization<Startup>();
            services.AddOpenIDConnect<SiteUser>(
                useDevelopmentCertificate: true,
                signingCertificate: null,
                encryptionCertificate: null,
                createSchema: true,
                options =>
                {
                    //options.RequireHttps = !_webHostingEnvironment.IsDevelopment();
                    var application = new OpenIDConnectApplication()
                    {
                        ClientId = "postman-client",
                        ClientSecret = "postman",
                        Scopes =
                        {
                            ServiceApiOptionsDefaults.Scope
                        }
                    };

                    // Using Postman for testing purpose.
                    // The authorization code is sent to postman after successful authentication.
                    application.RedirectUris.Add(new Uri("https://oauth.pstmn.io/v1/callback"));
                    options.Applications.Add(application);
                    options.AllowResourceOwnerPasswordFlow = true;

                    options.Applications.Add(new OpenIDConnectApplication()
                    {
                        ClientId = "anon-client",
                        Scopes = {
                            "anonymous_id"
                        }
                    });
                    options.AllowAnonymousFlow = true;
                });

            services.AddOpenIDConnectUI();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAnonymousId();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
            });
        }
    }
}