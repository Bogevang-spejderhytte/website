using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bogevang.Booking.Domain.CustomEntities
{
  public class TenantCategoryDataModel : ICustomEntityDataModel
  {
    [Display(Name = "Reservationstid (antal måneder i forvejen)", Description = "Antal måneder ud i fremtiden denne kategori af lejere må reservere.")]
    [Required]
    public int AllowedBookingFutureMonths { get; set; }

    [Display(Name = "Lejepris pr. døgn")]
    [Required]
    public decimal RentalPrice { get; set; }

    [Display(Name = "Rabatkode")]
    public string PromotionCode { get; set; }
  }
}
