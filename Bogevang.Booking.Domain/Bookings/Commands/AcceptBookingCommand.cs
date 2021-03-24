using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class AcceptBookingCommand : 
    ICommand
  {
    public int Id { get; set; }
  }
}
