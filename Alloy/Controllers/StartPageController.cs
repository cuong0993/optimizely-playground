using System.Globalization;
using Alloy.Models.Pages;
using Alloy.Models.ViewModels;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.DataAbstraction.RuntimeModel.Internal;
using EPiServer.Framework.Localization;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Uri = System.Uri;

namespace Alloy.Controllers;

public class StartPageController : PageControllerBase<StartPage>
{
    public IActionResult Index(StartPage currentPage)
    { 
        //Thread.CurrentThread.CurrentCulture 
        //ICurrentCultureAccessor
        var d = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        var e = CultureInfo.CurrentCulture;

        var numberDecimalDigits = NumberFormatInfo.CurrentInfo.NumberDecimalDigits;
        var a = new CultureInfo("en-AU");
        var numberDecimalDigitsAU = a.NumberFormat.NumberDecimalDigits;
        var b = CultureInfo.CurrentCulture;
        var numberDecimalDigitsCMS = b.NumberFormat.NumberDecimalDigits;
        var model = PageViewModel.Create(currentPage);

        // Check if it is the StartPage or just a page of the StartPage type.
        if (SiteDefinition.Current.StartPage.CompareToIgnoreWorkID(currentPage.ContentLink))
        {
            // Connect the view models logotype property to the start page's to make it editable
            var editHints = ViewData.GetEditHints<PageViewModel<StartPage>, StartPage>();
            editHints.AddConnection(m => m.Layout.Logotype, p => p.SiteLogotype);
            editHints.AddConnection(m => m.Layout.ProductPages, p => p.ProductPageLinks);
            editHints.AddConnection(m => m.Layout.CompanyInformationPages, p => p.CompanyInformationPageLinks);
            editHints.AddConnection(m => m.Layout.NewsPages, p => p.NewsPageLinks);
            editHints.AddConnection(m => m.Layout.CustomerZonePages, p => p.CustomerZonePageLinks);
        }
        GlobalTypeHandlers.Instance.Remove(typeof(Uri));
        //GlobalTypeHandlers.Instance.Remove(typeof(IEnumerable<Uri>));
        GlobalTypeHandlers.Instance.Add(typeof(Uri), new UriTypeHandler());
        //GlobalTypeHandlers.Instance.Add(typeof(IEnumerable<Uri>), new UriTypeHandler1());

       var store =  DynamicDataStoreFactory.Instance
            .CreateStore("ShippingAreas", typeof(ShippingArea));
       var bb = new ShippingArea();
      //bb.Id1 = "aaaaaaaaaaaaaaaaa";
       bb.Id2 = new Uri("http://www.MyHost/MyPageURL");
       //bb.MyCollection2 = new[] {"http://www.MyHost/MyPage2", "http://www.MyHost/MyPage3" };
       bb.MyCollection1 = new[] {new Uri("http://www.MyHost/MyPage0"), new Uri("http://www.MyHost/MyPage1") };
       store.Save(bb);

        return View(model);
    }
}

internal class UriTypeHandler : ITypeHandler
{
   // private readonly IHostApplication _host;
   
    #region ITypeHandler Members

    public Type MapToDatabaseType(Type type)
    {
       // _host.Assert(type == typeof(Uri));

        // I know how to map a Uri to a String
        return typeof(string);
    }

    public object ToDatabaseFormat(string propertyName, object propertyValue, Type ownerType)
    {
        //_host.Assert(propertyValue.GetType() == typeof(Uri));
        //_host.Out.WriteLine(string.Format("Converting property '{0}' of Type {1} from Uri to String", propertyName, ownerType.FullName));

        // Uri.ToString() outputs a string representation of the Uri which can be saved in the Dynamic Data Store
        var a = propertyValue.ToString();
        return a;
    }

    public object FromDatabaseFormat(string propertyName, object propertyValue, Type targetType, Type ownerType)
    {
       // _host.Assert(targetType == typeof(Uri));
       // _host.Out.WriteLine(string.Format("Converting property '{0}' of Type {1} from String to Uri", propertyName, ownerType.FullName));

        // Uri constructor takes a string
        return new Uri(propertyValue.ToString());
    }

    #endregion
}

internal class UriTypeHandler1 : ITypeHandler
{
    // private readonly IHostApplication _host;
   
    #region ITypeHandler Members

    public Type MapToDatabaseType(Type type)
    {
        // _host.Assert(type == typeof(Uri));

        // I know how to map a Uri to a String
        return typeof(string);
    }

    public object ToDatabaseFormat(string propertyName, object propertyValue, Type ownerType)
    {
        //_host.Assert(propertyValue.GetType() == typeof(Uri));
        //_host.Out.WriteLine(string.Format("Converting property '{0}' of Type {1} from Uri to String", propertyName, ownerType.FullName));

        // Uri.ToString() outputs a string representation of the Uri which can be saved in the Dynamic Data Store
        var a = propertyValue.ToString();
        return a;
    }

    public object FromDatabaseFormat(string propertyName, object propertyValue, Type targetType, Type ownerType)
    {
        // _host.Assert(targetType == typeof(Uri));
        // _host.Out.WriteLine(string.Format("Converting property '{0}' of Type {1} from String to Uri", propertyName, ownerType.FullName));

        // Uri constructor takes a string
        return new Uri(propertyValue.ToString());
    }

    #endregion
}

[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
internal class ShippingArea : IDynamicData
{
    public Identity Id { get; set; }
    public String Id1 { get; set; }
    public Uri Id2 { get; set; }

    public IEnumerable<String> MyCollection2 { get; set; }
    public IEnumerable<Uri> MyCollection1 { get; set; }

}

