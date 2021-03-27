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

      try
      {
        if (Cache == null)
        {
          var allEntities = await ContentRepository
            .CustomEntities()
            .GetByDefinitionCode(CustomEntityDefinitionCode)
            .AsRenderSummary()
            .ExecuteAsync();

          List<CacheEntry> entries = new List<CacheEntry>();
          foreach (var entity in allEntities)
            entries.Add(await MapEntity(entity));

          Cache = PostProcess(entries);
        }
      }
      finally
      {
        CacheSemaphore.Release();
      }
    }


    protected abstract string CustomEntityDefinitionCode { get; }

    protected abstract bool IsRelevantEntityCode(string entityCode);

    protected abstract Task<CacheEntry> MapEntity(CustomEntityRenderSummary entity);

    protected abstract List<CacheEntry> PostProcess(List<CacheEntry> entries);


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
