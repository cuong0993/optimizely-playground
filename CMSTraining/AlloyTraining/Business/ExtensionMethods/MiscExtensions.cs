using System;
using System.Text;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace AlloyTraining.Business.ExtensionMethods
{
    public static class MiscExtensions
    {
        /// <summary>
        ///     Truncate strings after n words
        /// </summary>
        /// <param name="input"></param>
        /// <param name="noWords"></param>
        /// <returns></returns>
        public static string TruncateAtWord(this string input, int noWords)
        {
            var output = string.Empty;
            var inputArr = input.Split(' ');
            if (inputArr.Length <= noWords)
                return input;
            if (noWords > 0)
            {
                for (var i = 0; i < noWords; i++) output += inputArr[i] + " ";
                output += "...";
                return output;
            }

            return input;
        }

        public static string ExternalURLFromReference(this PageReference p)
        {
            var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var page = loader.Get<PageData>(p);

            var pageURLBuilder = new UrlBuilder(page.LinkURL);

            Global.UrlRewriteProvider.ConvertToExternal(pageURLBuilder, page.PageLink, Encoding.UTF8);

            var pageURL = pageURLBuilder.ToString();

            var uriBuilder = new UriBuilder(SiteDefinition.Current.SiteUrl);

            uriBuilder.Path = pageURL;

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}