using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System.Web.Mvc;
using System.Web.Routing;

namespace AlloyDemo.Features.Favorites
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class FavoritesInitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(
                name: "FavoritesAdd", 
                url: "favs/add", 
                defaults: new { controller = "Favorites", action = "Add" });

            RouteTable.Routes.MapRoute(
                name: "FavoritesDelete",
                url: "favs/del",
                defaults: new { controller = "Favorites", action = "Delete" });
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}