using EPiServer.Shell.ObjectEditing;
using System.Collections.Generic;

namespace AlloyTraining.Business.SelectionFactories
{
    public class OgTypeSelectionFactory : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return new List<SelectItem>
            {
                new SelectItem { Value = "article", Text = "Article" },
                new SelectItem { Value = "book", Text = "Book" },
                new SelectItem { Value = "profile", Text = "Profile" },
                new SelectItem { Value = "website", Text = "Website" },
                new SelectItem { Value = "video.movie", Text = "Movie" },
                new SelectItem { Value = "video.episode", Text = "Episode" },
                new SelectItem { Value = "video.tv_show", Text = "TV Show" },
                new SelectItem { Value = "music.song", Text = "Song" },
                new SelectItem { Value = "music.album", Text = "Album" },
                new SelectItem { Value = "music.playlist", Text = "Playlist" },
                new SelectItem { Value = "music.radio_station", Text = "Radio Station" },
            };
        }
    }
}