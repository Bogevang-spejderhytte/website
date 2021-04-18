using Bogevang.Common.Utility;
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.TenantCategories.CustomEntities
{
  public class TenantCategoryDataModel : ICustomEntityDataModel, IActiveState
  {
    [Display(Name = "Reservationstid (antal måneder i forvejen)", Description = "Antal måneder ud i fremtiden denne kategori af lejere må reservere.")]
    [Required]
    public int AllowedBookingFutureMonths { get; set; }


    [Display(Name = "Aktiv (ja/nej). BRUGES I STEDET FOR AT SLETTE KATEGORIER!")]
    public bool IsActive { get; set; }


    public TenantCategoryDataModel()
    {
      IsActive = true;
    }
  }
}
