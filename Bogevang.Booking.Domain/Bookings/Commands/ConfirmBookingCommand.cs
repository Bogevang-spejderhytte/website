using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class ConfirmBookingCommand : 
    ICommand
  {
    public int Id { get; set; }
  }
}
