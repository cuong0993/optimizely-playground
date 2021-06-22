using AlloyTraining.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Filters;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.Web;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace AlloyTraining.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CreatePagesInitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // this returns empty if one of your sites does not have a * wildcard hostname
            ContentReference startReference = ContentReference.StartPage;

            // we can use the site definition repo to get the first site's start page instead
            if (ContentReference.IsNullOrEmpty(startReference))
            {
                var siteRepo = context.Locate.Advanced.GetInstance<ISiteDefinitionRepository>();
                var firstSite = siteRepo.List().FirstOrDefault();
                if (firstSite == null) return; // if no sites, give up running this module
                startReference = firstSite.StartPage;
            }

            string enabledString = ConfigurationManager.AppSettings["alloy:CreatePages"];
            bool enabled;
            if (bool.TryParse(enabledString, out enabled))
            {
                if (enabled)
                {
                    IContentRepository repo = context.Locate.Advanced.GetInstance<IContentRepository>();

                    // create About Us page
                    StandardPage aboutUs;

                    IContent content = repo.GetBySegment(
                        startReference, "about-us", 
                        CultureInfo.GetCultureInfo("en"));

                    if (content == null)
                    {
                        aboutUs = repo.GetDefault<StandardPage>(startReference);

                        aboutUs.Name = "About us";
                        aboutUs.MetaTitle = "About us title";
                        aboutUs.MetaDescription = "Alloy improves the effectiveness of project teams by putting the proper tools in your hands. Communication is made easy and inexpensive, no matter where team members are located.";
                        aboutUs.MainBody = new XhtmlString(aboutUs.MetaDescription);
                        aboutUs.SortIndex = 400;

                        repo.Save(aboutUs, SaveAction.Publish, AccessLevel.NoAccess);
                    }

                    // get the \For All Sites\Products\ folder
                    ContentFolder productsFolder = repo.GetBySegment(
                        ContentReference.GlobalBlockFolder, "Products",
                        CultureInfo.GetCultureInfo("en")) as ContentFolder;

                    // create Alloy Meet page
                    ProductPage alloyMeet;

                    content = repo.GetBySegment(
                        startReference, "alloy-meet",
                        CultureInfo.GetCultureInfo("en"));

                    if (content == null)
                    {
                        alloyMeet = repo.GetDefault<ProductPage>(startReference);

                        alloyMeet.Name = "Alloy Meet";
                        alloyMeet.MetaDescription = "You've never had a meeting like this before!";
                        alloyMeet.MainBody = new XhtmlString("Participants from remote locations appear in your meeting room, around your table, or stand presenting at your white board.");
                        alloyMeet.Theme = "theme1";
                        alloyMeet.UniqueSellingPoints = new[]
                        {
                            "Project tracking",
                            "White board sketch",
                            "Built-in reminders",
                            "Share meeting results",
                            "Email interface to request meetings"
                        };
                        alloyMeet.SortIndex = 100;
                        alloyMeet.PageImage = repo.GetBySegment(
                            productsFolder.ContentLink, "AlloyMeet.png",
                            CultureInfo.GetCultureInfo("en")).ContentLink;

                        repo.Save(alloyMeet, SaveAction.Publish, AccessLevel.NoAccess);
                    }

                    // create Alloy Plan page
                    ProductPage alloyPlan;

                    content = repo.GetBySegment(
                        startReference, "alloy-plan",
                        CultureInfo.GetCultureInfo("en"));

                    if (content == null)
                    {
                        alloyPlan = repo.GetDefault<ProductPage>(startReference);

                        alloyPlan.Name = "Alloy Plan";
                        alloyPlan.MetaDescription = "Project management has never been easier!";
                        alloyPlan.MainBody = new XhtmlString("Planning is crucial to the success of any project. Alloy Plan takes into consideration all aspects of project planning; from well-defined objectives to staffing, capital investments and management support. Nothing is left to chance.");
                        alloyPlan.Theme = "theme2";
                        alloyPlan.UniqueSellingPoints = new[]
                        {
                            "Project planning",
                            "Reporting and statistics",
                            "Email handling of tasks",
                            "Risk calculations",
                            "Direct communication to members"
                        };
                        alloyPlan.SortIndex = 200;
                        alloyPlan.PageImage = repo.GetBySegment(
                            productsFolder.ContentLink, "AlloyPlan.png",
                            CultureInfo.GetCultureInfo("en")).ContentLink;

                        repo.Save(alloyPlan, SaveAction.Publish, AccessLevel.NoAccess);
                    }

                    // create Alloy Track page
                    ProductPage alloyTrack;

                    content = repo.GetBySegment(
                        startReference, "alloy-track",
                        CultureInfo.GetCultureInfo("en"));

                    if (content == null)
                    {
                        alloyTrack = repo.GetDefault<ProductPage>(startReference);

                        alloyTrack.Name = "Alloy Track";
                        alloyTrack.MetaDescription = "Projects have a natural lifecycle with well-defined stages.";
                        alloyTrack.MainBody = new XhtmlString("From start-up meetings to final sign-off, we have the solutions for today’s market-driven needs. Leverage your assets to the fullest through the combination of Alloy Plan, Alloy Meet and Alloy Track.");
                        alloyTrack.Theme = "theme3";
                        alloyTrack.UniqueSellingPoints = new[]
                        {
                            "Shared timeline",
                            "Project emails",
                            "To-do lists",
                            "Workflows",
                            "Status reports"
                        };
                        alloyTrack.SortIndex = 300;
                        alloyTrack.PageImage = repo.GetBySegment(
                            productsFolder.ContentLink, "AlloyTrack.png",
                            CultureInfo.GetCultureInfo("en")).ContentLink;

                        repo.Save(alloyTrack, SaveAction.Publish, AccessLevel.NoAccess);
                    }

                    // change Start page sort order for children
                    if (repo.Get<StartPage>(startReference)
                        .CreateWritableClone() is StartPage startPage)
                    {
                        startPage.ChildSortOrder = FilterSortOrder.Index;
                        repo.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}