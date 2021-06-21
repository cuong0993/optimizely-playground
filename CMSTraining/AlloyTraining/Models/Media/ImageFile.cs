using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace AlloyTraining.Models.Media
{
    [ContentType(DisplayName = "ImageFile", GUID = "ff0396c9-1c36-4e46-a298-fe715cd93b41", Description = "Use this to upload image files.”")]
    [MediaDescriptor(ExtensionString = "png,gif,jpg,jpeg")]
    public class ImageFile : ImageData
    {
        
                [CultureSpecific]
                [Editable(true)]
                public virtual string Description { get; set; }
         
    }
}