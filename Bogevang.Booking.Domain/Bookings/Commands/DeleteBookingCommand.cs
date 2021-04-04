using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class DeleteBookingCommand : ICommand
  {
    public int Id { get; set; }
  }
}
