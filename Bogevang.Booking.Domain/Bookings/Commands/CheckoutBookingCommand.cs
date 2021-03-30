
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class CheckoutBookingCommand : ICommand
  {
    public int Id { get; set; }

    public string Token { get; set; }

    [Display(Name = "Aflæsning af el-måler ved ankomst (kWh)")]
    [Required]
    public decimal? StartReading { get; set; }

    [Display(Name = "Aflæsning af el-måler ved afrejse (kWh)")]
    [Required]
    public decimal? EndReading { get; set; }

    [Display(Name = "Bemærkninger")]
    public string Comments { get; set; }
  }
}
