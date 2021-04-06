using Cofoundry.Domain.CQS;
using System.IO;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class ImportBookingsCommand : ICommand
  {
    public Stream ReadyToReadInput { get; set; }
  }
}
