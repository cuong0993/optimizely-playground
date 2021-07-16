using System.Collections.Generic;
using EPiServer.Core;

namespace AlloyDemo.Features.Favorites
{
    public class FavoriteViewModel
    {
        public List<Favorite> Favorites { get; set; }
        public ContentReference CurrentPageContentReference { get; set; }
    }
}