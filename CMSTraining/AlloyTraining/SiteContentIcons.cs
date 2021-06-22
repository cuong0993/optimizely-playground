using EPiServer.DataAnnotations;

namespace AlloyTraining
{
    public class SiteImageUrlAttribute : ImageUrlAttribute
    {
        public SiteImageUrlAttribute()
            : base("~/Static/contenticons/epi-edu-icon.jpg") { }

        public SiteImageUrlAttribute(string path)
            : base(path) { }
    }

    public class SitePageIconAttribute : ImageUrlAttribute
    {
        public SitePageIconAttribute()
            : base("~/Static/contenticons/epi-edu-icon-page.jpg") { }
    }

    public class SiteBlockIconAttribute : ImageUrlAttribute
    {
        public SiteBlockIconAttribute()
            : base("~/Static/contenticons/epi-edu-icon-block.jpg") { }
    }

    public class SiteStartIconAttribute : ImageUrlAttribute
    {
        public SiteStartIconAttribute()
            : base("~/Static/contenticons/epi-edu-icon-start.jpg") { }
    }

    public class SiteSearchIconAttribute : ImageUrlAttribute
    {
        public SiteSearchIconAttribute()
            : base("~/Static/contenticons/epi-edu-icon-search.jpg") { }
    }

    public class SiteCommerceIconAttribute : ImageUrlAttribute
    {
        public SiteCommerceIconAttribute()
            : base("~/Static/contenticons/epi-edu-icon-commerce.jpg") { }
    }

    public class SiteContainerIconAttribute : ImageUrlAttribute
    {
        public SiteContainerIconAttribute()
            : base("~/Static/contenticons/epi-edu-icon-container.jpg") { }
    }
}