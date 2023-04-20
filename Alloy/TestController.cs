using Alloy.Models.Pages;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace Foundation
{
    public class TestController : Controller
    {
        public object Index()
        {
            return "Hello";
        }

        public object ListSortedPages(int id, string culture = "en")
        {
            var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var list = new List<object>();
            var items = loader.GetChildren<ArticlePage>(new ContentReference(id), CultureInfo.GetCultureInfo(culture));
            foreach (var item in items)
            {
                list.Add(new { ID = item.ContentLink.ID, Name = item.Name, Order = item.SortIndex, Modified = item.Changed.ToString() });
            }
            //return items.Count().ToString();
            return JsonConvert.SerializeObject(list, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}
