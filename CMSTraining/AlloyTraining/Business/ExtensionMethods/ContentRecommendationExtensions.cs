using System.Web;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace AlloyTraining.Business.ExtensionMethods
{
    public static class ContentRecommendationExtensions
    {
        /* https://ogp.me/#type_article
        <meta property="og:type" content="article">
        <meta property="og:title" content="Alloy Meet">
        <meta property="og:site_name" content="Alloy Training">
        <meta property="og:url" content="/en/alloy-meet">
        <meta property="og:image" content="/alloy-meet.jpeg">
        <meta property="article:published_time" content="2019-12-18">
        <meta property="article:author" content="">
         */

        public static IHtmlString OpenGraphMetaTags(
            this HtmlHelper html,
            ContentReference contentLink = null)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var siteDefinition = ServiceLocator.Current.GetInstance<ISiteDefinitionResolver>();

            var requestContext = html.ViewContext.RequestContext;
            if (contentLink == null) contentLink = requestContext.GetContentLink();

            var siteName = siteDefinition.GetByContent(contentLink, true).Name;

            if (contentLoader.TryGet(contentLink, out PageData pageData))
            {
                // in Alloy it should find MetaTitle first
                var title = pageData.GetFirstMatchingProperty(
                    new[] {"OgTitle", "MetaTitle", "PageName"});

                // in Alloy it won't find any of these
                var type = pageData.GetFirstMatchingProperty(
                    new[] {"OgType", "MetaPageType"});
                if (type == null) type = "article";

                var author = pageData.GetFirstMatchingProperty(
                    new[] {"OgAuthor", "Author", "PageCreatedBy"});

                // in Alloy it should find PageImage and use it if set
                var imageRef = pageData.GetFirstMatchingProperty(
                    new[] {"OgImage", "PageImage", "Image"});

                var image = string.Empty;
                if (ContentReference.TryParse(imageRef, out var contentReference))
                    image = GetExternalUrl(contentReference);

                var url = GetExternalUrl(contentLink);

                var pubDate = pageData.StartPublish?.ToString("yyyy-MM-dd");

                var titleTag = $"<meta property=\"og:title\" content=\"{title}\" />";
                var siteTag = $"<meta property=\"og:site_name\" content=\"{siteName}\" />";
                var typeTag = $"<meta property=\"og:type\" content=\"{type}\" />";
                var imageTag = $"<meta property=\"og:image\" content=\"{image}\" />";
                var urlTag = $"<meta property=\"og:url\" content=\"{url}\" />";
                var authorTag = $"<meta property=\"article:author\" content=\"{author}\" />";
                var pubDateTag = $"<meta property=\"article:published_time\" content=\"{pubDate}\" />";

                return new MvcHtmlString(string.Join("\n    ",
                    "<!-- meta tags to improve Content Recommendations -->",
                    titleTag, siteTag, typeTag, urlTag, imageTag, pubDateTag, authorTag));
            }

            return MvcHtmlString.Empty;
        }

        private static string GetFirstMatchingProperty(this IContentData data, string[] names)
        {
            foreach (var name in names)
                if (data.Property[name] != null)
                    if (data.Property[name].Value != null)
                        return data.Property[name].Value.ToString();
            return null;
        }

        private static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;

            return string.Format(
                "{0}://{1}",
                request.Url.Scheme,
                request.Url.Authority
            );
        }

        private static string GetExternalUrl(ContentReference contentLink)
        {
            var baseUrl = GetBaseUrl();

            if (baseUrl.EndsWith("/")) baseUrl = baseUrl.TrimEnd('/');

            var contentPath = ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(contentLink);

            if (!contentPath.StartsWith("/")) contentPath += '/';

            return string.Concat(baseUrl, contentPath);
        }
    }
}