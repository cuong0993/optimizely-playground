using EPiServer.Core;

namespace AlloyTraining.Models.ViewModels
{
    public class ContentFolderViewModel
    {
        public ContentFolder CurrentFolder { get; set; }
        public int ItemsCount { get; set; }
    }
}