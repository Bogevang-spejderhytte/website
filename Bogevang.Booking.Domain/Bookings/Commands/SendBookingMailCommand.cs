using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class SendBookingMailCommand : ICommand
  {
    [Required]
    public int BookingId { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string Message { get; set; }
  }
}
