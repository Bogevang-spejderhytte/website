using System;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Website.ViewModels
{
  public class BookingCheckoutViewModel
  {
    [Display(Name = "Aftalenr.")]
    public int Id { get; set; }

    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public string ContactName { get; set; }

  }
}
