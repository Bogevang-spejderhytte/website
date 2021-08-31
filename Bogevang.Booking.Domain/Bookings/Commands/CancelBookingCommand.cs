using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class CancelBookingCommand : ICommand
  {
    public int Id { get; set; }
  }
}
