using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class AnonymizeBookingsCommand : ICommand
  {
    [OutputValue]
    public int AnonymizedCount { get; set; }
  }
}
