using EPiServer.Core;
using System.Collections.Generic;

namespace AlloyDemo.Features.Favorites
{
    public class FavoriteViewModel
    {
        public List<Favorite> Favorites { get; set; }
        public ContentReference CurrentPageContentReference { get; set; }
    }
}