// Decompiled with JetBrains decompiler
// Type: EPiServer.Cms.WelcomeIntegration.Core.Internal.DAMOutgoingUrlRewriter
// Assembly: EPiServer.Cms.WelcomeIntegration.Core, Version=1.3.8.0, Culture=neutral, PublicKeyToken=null
// MVID: B9C5C788-E9B5-4A44-BCAB-A1BAEFB495D7
// Assembly location: /Users/cuongnguyen/.nuget/packages/episerver.cms.welcomeintegration.core/1.3.8/lib/net6.0/EPiServer.Cms.WelcomeIntegration.Core.dll

using EPiServer.Core;
using EPiServer.Core.Routing;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using OptimizelyPublicUrlIssue.BLL;
using OptimizelyPublicUrlIssue.BLL.CmsUnits;

#nullable enable
namespace Alloy
{
  internal class DAMOutgoingUrlRewriter : IHostedService
  {
    private readonly IContentUrlGeneratorEvents _contentUrlGeneratorEvents;
    private readonly IContentLoader _contentLoader;

    public DAMOutgoingUrlRewriter(
      IContentUrlGeneratorEvents contentUrlGeneratorEvents,
      IContentLoader contentLoader)
    {
      this._contentUrlGeneratorEvents = contentUrlGeneratorEvents;
      this._contentLoader = contentLoader;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      this._contentUrlGeneratorEvents.GeneratingUrl += new EventHandler<UrlGeneratorEventArgs>(this.GenerateDAMUrl);
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      this._contentUrlGeneratorEvents.GeneratingUrl -= new EventHandler<UrlGeneratorEventArgs>(this.GenerateDAMUrl);
      return Task.CompletedTask;
    }

    private void GenerateDAMUrl(object? sender, UrlGeneratorEventArgs e)
    {
      if (!"optimizely-public-url-issue-assets-provider".Equals(e.Context.ContentLink?.ProviderName, StringComparison.OrdinalIgnoreCase) || !(this._contentLoader.Get<IContent>(e.Context.ContentLink) is OptimizelyPublicUrlIssueAssetImageData damAsset))
        return;
      e.Context.Url = new UrlBuilder(damAsset.PreviewUrl);
      e.AppendTrailingSlash = false;
      e.State = RoutingState.Done;
    }
  }
}
