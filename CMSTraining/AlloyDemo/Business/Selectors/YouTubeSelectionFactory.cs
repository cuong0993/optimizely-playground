using System.Collections.Generic;
using AlloyDemo.Models;
using EPiServer.Shell.ObjectEditing;

namespace AlloyDemo.Business.Selectors
{
    public class YouTubeSelectionFactory : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return EpiserverYouTube.Videos;
        }
    }
}