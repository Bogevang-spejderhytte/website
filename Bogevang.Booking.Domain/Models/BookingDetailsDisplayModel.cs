using Bogevang.Booking.Domain.CustomEntities;
using Cofoundry.Domain;
using System;

namespace Bogevang.Booking.Domain.Models
{
  public class BookingDetailsDisplayModel : ICustomEntityPageDisplayModel<BookingDataModel>
  {
    public DateTime? ArrivalDate { get; set; }
    public DateTime? DepartureDate { get; set; }
    public string Purpose { get; set; }
    public string TenantName { get; set; }
    public string ContactName { get; set; }
    public string ContactEMail { get; set; }

    public string PageTitle { get; set; }
    public string MetaDescription { get; set; }
  }
}
