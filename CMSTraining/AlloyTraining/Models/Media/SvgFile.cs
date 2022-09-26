using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;

namespace AlloyTraining.Models.Media
{
    [ContentType(DisplayName = "SVG File", GUID = "4e9118e6-45ae-49e7-b55c-598ca5e29346",
        Description = "Use this to upload Scalable Vector Graphic (SVG) images.")]
    [MediaDescriptor(ExtensionString = "svg")]
    public class SvgFile : ImageData
    {
        // instead of generating a smaller bitmap file for thumbnail,
        // use the same binary vector image for thumbnail
        public override Blob Thumbnail => base.BinaryData;
    }
}