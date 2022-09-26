using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace AlloyDemo.Features.Favorites
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class FavoritesInitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(
                "FavoritesAdd",
                "favs/add",
                new {controller = "Favorites", action = "Add"});

            RouteTable.Routes.MapRoute(
                "FavoritesDelete",
                "favs/del",
                new {controller = "Favorites", action = "Delete"});
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}