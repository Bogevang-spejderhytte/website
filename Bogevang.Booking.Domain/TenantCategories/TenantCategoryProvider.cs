using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Cofoundry.Domain;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.TenantCategories
{
  public class TenantCategoryProvider : ITenantCategoryProvider
  {
    private readonly IAdvancedContentRepository ContentRepository;


    public TenantCategoryProvider(
      IAdvancedContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }


    public async Task<TenantCategoryDataModel> GetTenantCategoryById(int id)
    {
      var entity = await ContentRepository
            .CustomEntities()
            .GetById(id)
            .AsRenderSummary()
            .ExecuteAsync();

      TenantCategoryDataModel category = (TenantCategoryDataModel)entity.Model;

      return category;
    }
  }
}
