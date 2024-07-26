using Cofoundry.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Website.ViewModels
{
  public class BookingCheckoutViewModel
  {
    public int BookingId { get; set; }

    [Display(Name = "Aftalenr.")]
    public int BookingNumber { get; set; }

    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public string ContactName { get; set; }

    [Display(Name = "Aflæsning af el-måler ved ankomst (kWh)")]
    public decimal StartReading { get; set; }

    [Display(Name = "Aflæsning af el-måler ved afrejse (kWh)")]
    public decimal EndReading { get; set; }

    public decimal Deposit { get; set; }

    [Display(Name = "Bemærkninger", Description = "Har du nogen bemærkninger til opholdet eller ideer til forbedringer, så skriv dem her.")]
    [MultiLineText(Rows = 6)]
    [MaxLength(2000)]
    public string Comments { get; set; }

    public decimal ElectricityPriceUnit { get; set; }
  }
}
