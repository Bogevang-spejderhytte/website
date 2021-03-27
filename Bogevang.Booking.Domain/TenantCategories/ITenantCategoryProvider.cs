using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.TenantCategories
{
  public interface ITenantCategoryProvider
  {
    Task<TenantCategoryDataModel> GetTenantCategoryById(int id);
  }
}
