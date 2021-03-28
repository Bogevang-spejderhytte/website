using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class RejectBookingCommand : ICommand
  {
    public int Id { get; set; }
  }
}
