using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using AlloyDemo.Models.Media;
using AlloyDemo.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace AlloyDemo.Business.ScheduledJobs
{
    [ScheduledPlugIn(DisplayName = ScheduledJobName,
        GUID = "88E83CA0-91ED-46FD-A338-7938C8D0FDF9",
        SortIndex = -1, Restartable = true)]
    public class ImportImagesScheduledJob : ScheduledJobBase
    {
        public const string ScheduledJobName = "Import Images";
        private readonly IBlobFactory blobFactory;

        private readonly IContentRepository contentRepository;

        private readonly string[] patterns = { "*.png", "*.jpeg", "*.jpg" };

        private bool _stopSignaled;

        public ImportImagesScheduledJob()
        {
            IsStoppable = true;
        }

        public ImportImagesScheduledJob(
            IContentRepository contentRepository,
            IBlobFactory blobFactory) : this()
        {
            this.contentRepository = contentRepository;
            this.blobFactory = blobFactory;
        }

        public override void Stop()
        {
            _stopSignaled = true;
        }

        private IEnumerable<string> GetImageFilenames(string path)
        {
            IEnumerable<string> files = null;
            foreach (var pattern in patterns)
            {
                IEnumerable<string> moreFiles = Directory.GetFiles(path, pattern);
                if (files == null) files = moreFiles;
                else files.Concat(moreFiles);
            }

            return files;
        }

        public override string Execute()
        {
            //var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            //var contentPage = repository.GetDefault<SearchPage>(ContentReference.StartPage);

            //contentPage.Name = "Example Page";
            //contentPage.RelatedContentArea = new ContentArea();

            //repository.Save(contentPage,
            //SaveAction.Publish,
            //AccessLevel.NoAccess);

            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var existingContentPage = repository
                .GetChildren<ProductPage>(ContentReference.StartPage);

            var existingContentPage1 = repository.GetChildren<ProductPage>(ContentReference.StartPage, CultureInfo.GetCultureInfo("sv"));


            var contentLoader = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IContentLoader>();
            var pages = contentLoader.GetChildren<PageData>(ContentReference.StartPage);
            var pages1 = contentLoader.Get<IContent>(new Guid("0e47c41e-9d6b-465e-872c-18c05d5b61d7"));
            var pages2 = contentLoader.GetChildren<ProductPage>(new ContentReference(192));
            var contentReference = new ContentReference(192);

            var pages5 = contentLoader.Get<IContent>(new ContentReference(45));
            var pages3 = contentLoader.Get<IContent>(new ContentReference(47));
            var pages6 = contentLoader.Get<IContent>(new ContentReference(195));

            var pages4 = contentLoader.Get<IContent>(new ContentReference(196));

            var contentVersionRepository = ServiceLocator.Current.GetInstance<IContentVersionRepository>();
            var versions = contentVersionRepository.List(contentReference);
            var versions1 = contentVersionRepository.List(new ContentReference(47));
            var versions2 = contentVersionRepository.List(new ContentReference(196));
            versions.OrderByDescending(x => x.Saved).FirstOrDefault(version => version.IsMasterLanguageBranch);
            int a = 0;

            var toImportFolder = WebConfigurationManager.AppSettings["episerver:edu.ToImportFolder"];
            var importedFolder = WebConfigurationManager.AppSettings["episerver:edu.ImportedFolder"];

            var assetsFolder = new ContentReference(
                WebConfigurationManager.AppSettings["episerver:edu.ImportAssetsFolder"]);

            var images = GetImageFilenames(toImportFolder);
            var toImportCount = images.Count();
            var importedCount = 0;
            var remainingCount = toImportCount;

            OnStatusChanged($"Starting {ScheduledJobName}. {toImportCount} images to import. Please wait...");

            while (remainingCount > 0)
            {
                if (_stopSignaled) return "Stop of job was called";

                var nextImage = images.First();

                var asset = contentRepository.GetDefault<ImageFile>(assetsFolder);
                asset.Name = Path.GetFileName(nextImage);
                asset.Copyright = "Copyright © 2018 Episerver Education";

                var blob = blobFactory.CreateBlob(asset.BinaryDataContainer,
                    Path.GetExtension(nextImage));
                blob.WriteAllBytes(File.ReadAllBytes(nextImage));
                asset.BinaryData = blob;

                contentRepository.Save(asset, SaveAction.Publish, AccessLevel.NoAccess);

                File.Move(nextImage, Path.Combine(
                    importedFolder, Path.GetFileName(nextImage)));

                Thread.Sleep(2500); // slow it down
                importedCount++;

                OnStatusChanged($"Imported {importedCount} of {toImportCount} images. Please wait...");

                images = GetImageFilenames(toImportFolder);
                remainingCount = images.Count();
            }

            return $"Successfully imported {importedCount} images.";
        }
    }
}