using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using Foundation.Features.Error.Pages.ErrorDetail;
using Foundation.Features.ViewModels;
using Foundation.Features.ViewModels.Pages.Content;

namespace Foundation.Features.Shared.Error
{
    public class ErrorController : Controller
    {
        private static readonly Lazy<IContentTypeRepository> _contentTypeRepository = new Lazy<IContentTypeRepository>(() => ServiceLocator.Current.GetInstance<IContentTypeRepository>());
        private static readonly Lazy<IPublishedStateAssessor> _publishedStateAssessor = new Lazy<IPublishedStateAssessor>(() => ServiceLocator.Current.GetInstance<IPublishedStateAssessor>());

        [HttpGet]
        public ActionResult Index(Exception exception)
        {
            var errorDetailPage = GetPages<ErrorDetailPage>()
                .SingleOrDefault(x => x.ErrorCode == HttpContext.Response.StatusCode); // Find an error detail page with the corresponding error response code

            if (errorDetailPage != null) // If it finds it, show that page. If it does not, show the generic one.
            {
                return View("~/Features/Error/Pages/ErrorDetail/Index.cshtml", ContentViewModel.Create(errorDetailPage));
            }

            var model = new UserViewModel
            {
                Title = "Error",
                ErrorMessage = exception?.Message ?? "Not found",
                StackTrace = exception?.StackTrace ?? "Not stacktrace",
                Logo = ""
            };
            return View("~/Features/Shared/Error/ErrorPage.cshtml", model);
        }

        public static IEnumerable<T> GetPages<T>() where T : PageData
        {
            var startPage = ContentReference.StartPage;
            if (startPage == null || startPage.ID == 0)
            {
                return new List<T>();
            }

            // Define a criteria collection to do the search
            var criterias = new PropertyCriteriaCollection();

            // Create criteria for searching page types
            var criteria = new PropertyCriteria
            {
                Condition = CompareCondition.Equal,
                Type = PropertyDataType.PageType,
                Name = "PageTypeID",
                Value = _contentTypeRepository.Value.Load<T>().ID.ToString()
            };

            // Add criteria to collection
            criterias.Add(criteria);

            // Searching from Start Page
            var repository = ServiceLocator.Current.GetInstance<IPageCriteriaQueryService>();
            var pages = repository.FindPagesWithCriteria(startPage, criterias);

            var result = pages != null && pages.Any() ? pages
                .Where(IsContentPublished)
                .Select(z => (T)z) : new List<T>();

            return result;
        }

        public static bool IsContentPublished(IContent content)
        {
            return _publishedStateAssessor.Value.IsPublished(content, PagePublishedStatus.Published);
        }
    }
}