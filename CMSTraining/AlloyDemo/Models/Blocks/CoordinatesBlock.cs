using TedGustaf.Episerver.GoogleMapsEditor.Models;

namespace AlloyDemo.Models.Blocks
{
    public class CoordinatesBlock : SiteBlockData, IGoogleMapsCoordinates
    {
        public virtual double Longitude { get; set; }
        public virtual double Latitude { get; set; }
    }
}