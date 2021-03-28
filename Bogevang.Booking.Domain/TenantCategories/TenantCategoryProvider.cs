using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Core;
using Cofoundry.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.TenantCategories
{
  public class TenantCategoryProvider :
    CachedCustomEntityProvider<TenantCategoryDataModel, TenantCategoryDataModel>,
    ITenantCategoryProvider
  {
    public TenantCategoryProvider(
      IAdvancedContentRepository contentRepository)
      : base(contentRepository)
    {
    }


    protected override string CustomEntityDefinitionCode => TenantCategoryCustomEntityDefinition.DefinitionCode;


    protected override bool IsRelevantEntityCode(string entityCode)
    {
      return entityCode == TenantCategoryCustomEntityDefinition.DefinitionCode;
    }


    protected override CacheEntry MapEntity(CustomEntityRenderSummary entity)
    {
      var model = (TenantCategoryDataModel)entity.Model;

      return new CacheEntry
      {
        Entity = entity,
        DataModel = model,
        Summary = model
      };
    }


    public async Task<TenantCategoryDataModel> GetTenantCategoryById(int id)
    {
      await EnsureCacheLoaded();

      var result = Cache.FirstOrDefault(b => b.Entity.CustomEntityId == id);
      if (result == null)
        throw new EntityNotFoundException($"No tenant category with ID {id}.");
      return result.DataModel;
    }
  }
}
