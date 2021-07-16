using AlloyDemo.Models;
using EPiServer.Shell.ObjectEditing;
using System.Collections.Generic;

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