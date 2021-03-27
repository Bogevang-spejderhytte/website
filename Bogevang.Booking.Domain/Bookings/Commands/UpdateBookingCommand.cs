using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class UpdateBookingCommand : BookingDataModel, ICommand
  {
    public int BookingId { get; set; }
  }
}
