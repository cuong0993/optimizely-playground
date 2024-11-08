﻿using System.Linq;
using AlloyDemo.Models.NorthwindEntities;
using AlloyDemo.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;

namespace AlloyDemo.Business.ScheduledJobs
{
    [ScheduledPlugIn(DisplayName = "Import Shippers",
        GUID = "88E83CA0-92ED-46FD-A338-7938C8D0FDF9", SortIndex = -1)]
    public class ImportShippersScheduledJob : ScheduledJobBase
    {
        private readonly IContentRepository repo;
        private bool _stopSignaled;

        public ImportShippersScheduledJob()
        {
            IsStoppable = true;
        }

        public ImportShippersScheduledJob(IContentRepository repo) : this()
        {
            this.repo = repo;
        }

        public override void Stop()
        {
            _stopSignaled = true;
        }

        public override string Execute()
        {
            var shippersImported = 0;

            OnStatusChanged(
                "Starting execution of 'Import Shippers' job.");

            var startPage = repo.Get<StartPage>(ContentReference.StartPage);

            var existingShippers = repo.GetChildren<ShipperPage>(startPage.Shippers);

            var existingIDs = existingShippers.Select(s => s.ShipperID).ToArray();

            var db = new Northwind();

            var shippers = db.Shippers
                .Where(s => !existingIDs.Contains(s.ShipperID));

            foreach (var item in shippers)
            {
                var newshipper = repo.GetDefault<ShipperPage>(startPage.Shippers);

                newshipper.Name = item.CompanyName;
                newshipper.ShipperID = item.ShipperID;
                newshipper.CompanyName = item.CompanyName;
                newshipper.Phone = item.Phone;

                repo.Save(newshipper,
                    SaveAction.Publish,
                    AccessLevel.NoAccess);

                shippersImported++;

                if (_stopSignaled) return "'Import Shippers' job was stopped.";
            }

            if (shippersImported == 0)
                return "No new shippers to import.";
            return string.Format(
                "Successfully imported {0} shippers.",
                shippersImported);
        }
    }
}