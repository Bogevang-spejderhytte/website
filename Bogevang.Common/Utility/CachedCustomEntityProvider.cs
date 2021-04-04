using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bogevang.Common.Utility
{
  public abstract class CachedCustomEntityProvider<TModel, TSummary> :
    IMessageHandler<CustomEntityAddedMessage>,
    IMessageHandler<ICustomEntityContentUpdatedMessage>,
    IMessageHandler<CustomEntityDeletedMessage>
  {
    private readonly IAdvancedContentRepository ContentRepository;


    static SemaphoreSlim CacheSemaphore = new SemaphoreSlim(1, 1);

    protected class CacheEntry
    {
      public CustomEntityRenderSummary Entity { get; set; }
      public TModel DataModel { get; set; }
      public TSummary Summary { get; set; }
    }

    protected static List<CacheEntry> Cache { get; set; }


    public CachedCustomEntityProvider(
      IAdvancedContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }


    protected async Task EnsureCacheLoaded()
    {
      await CacheSemaphore.WaitAsync();

      bool doPostProcess = false;
      try
      {
        if (Cache == null)
        {
          var allEntities = await ContentRepository
            .WithElevatedPermissions()
            .CustomEntities()
            .GetByDefinitionCode(CustomEntityDefinitionCode)
            .AsRenderSummary()
            .ExecuteAsync();

          Cache = allEntities.Select(e => MapEntity(e)).ToList();
          doPostProcess = true;
        }
      }
      finally
      {
        CacheSemaphore.Release();
      }

      // Post processing may involved a callback to EnsureCacheLoad, which will block on the semaphore,
      // so wait until semaphore is released.
      if (doPostProcess)
        await PostProcessCache();
    }


    protected abstract string CustomEntityDefinitionCode { get; }

    protected abstract bool IsRelevantEntityCode(string entityCode);

    protected abstract CacheEntry MapEntity(CustomEntityRenderSummary entity);

    protected virtual Task PostProcessCache() => Task.CompletedTask;


    private async Task ResetCache()
    {
      await CacheSemaphore.WaitAsync();

      try
      {
        Cache = null;
      }
      finally
      {
        CacheSemaphore.Release();
      }
    }


    #region Message handlers for keeping cache in sync

    public async Task HandleAsync(CustomEntityAddedMessage message)
    {
      if (IsRelevantEntityCode(message.CustomEntityDefinitionCode))
        await ResetCache();
    }


    public async Task HandleAsync(ICustomEntityContentUpdatedMessage message)
    {
      if (IsRelevantEntityCode(message.CustomEntityDefinitionCode))
        await ResetCache();
    }


    public async Task HandleAsync(CustomEntityDeletedMessage message)
    {
      if (IsRelevantEntityCode(message.CustomEntityDefinitionCode))
        await ResetCache();
    }

    #endregion
  }
}
