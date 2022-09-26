using System;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace AlloyDemo.Features.Favorites
{
    public class Favorite : IDynamicData
    {
        public Favorite()
        {
            Id = Identity.NewIdentity(Guid.NewGuid());
            UserName = string.Empty;
            FavoriteContentReference = ContentReference.EmptyReference;
        }

        public Favorite(ContentReference contentReferenceToAdd,
            string userName) : this() // calls the default constructor first
        {
            UserName = userName;
            FavoriteContentReference = contentReferenceToAdd;
        }

        public string UserName { get; set; }
        public ContentReference FavoriteContentReference { get; set; }
        public Identity Id { get; set; }
    }
}