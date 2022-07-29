using System.Web.Mvc;

namespace MSWebPlayground.Controllers
{
    public class ErrorController : Controller
    {
        public string Index()
        {
            return "This is an error";
        }
    }
}