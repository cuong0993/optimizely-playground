using AlloyDemo.Models.NorthwindEntities;
using EPiServer.Logging;
using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlloyDemo.Business.Selectors
{
    public class CitySelectionFactory : ISelectionFactory
    {
        private static readonly ILogger logger =
            LogManager.GetLogger(typeof(CitySelectionFactory));

        private static List<SelectItem> list = null;

        public IEnumerable<ISelectItem> GetSelections(
            ExtendedMetadata metadata)
        {
            if (list == null)
            {
                list = new List<SelectItem>();
            }
            if (list.Count == 0)
            {
                var db = new Northwind();

                try
                {
                    list = db.Customers
                        .Select(customer => customer.City)
                        .Distinct()
                        .OrderBy(c => c)
                        .Select(city => new SelectItem
                        {
                            Text = city,
                            Value = city
                        })
                        .ToList();
                }
                catch (Exception ex)
                {
                    logger.Error($"{ex.GetType()}: {ex.Message}");
                }
            }
            return list;
        }
    }
}