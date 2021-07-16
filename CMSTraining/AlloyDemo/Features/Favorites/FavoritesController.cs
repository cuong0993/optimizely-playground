using System.Web.Mvc;
using AlloyDemo.Models.Pages;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Web.Routing;

namespace AlloyDemo.Features.Favorites
{
    public class FavoritesController : Controller
    {
        private readonly UrlResolver urlResolver;

        public FavoritesController(UrlResolver urlResolver)
        {
            this.urlResolver = urlResolver;
        }

        public ActionResult Manager(SitePageData currentPage)
        {
            var model = new FavoriteViewModel();

            model.Favorites = FavoriteRepository
                .GetFavorites(PrincipalInfo.Current.Name);

            model.CurrentPageContentReference = currentPage.ContentLink;

            return PartialView("~/Features/Favorites/FavoritesMenu.cshtml", model);
        }

        public void Add(ContentReference page)
        {
            var favorite = FavoriteRepository.GetFavorite(
                page, PrincipalInfo.Current.Name);

            if (favorite == null)
            {
                var newFavorite = new Favorite(page, PrincipalInfo.Current.Name);
                FavoriteRepository.Save(newFavorite);
            }

            Response.Redirect(urlResolver.GetUrl(page));
        }

        public void Delete(ContentReference page, ContentReference fav)
        {
            var favorite = FavoriteRepository.GetFavorite(
                fav, PrincipalInfo.Current.Name);

            if (favorite != null) FavoriteRepository.Delete(favorite);

            Response.Redirect(urlResolver.GetUrl(page));
        }
    }
}