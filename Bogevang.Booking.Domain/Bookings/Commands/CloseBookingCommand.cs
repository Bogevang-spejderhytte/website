using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class CloseBookingCommand : ICommand
  {
    public int Id { get; set; }
  }
}
