using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class SendWelcomeLetterCommand : ICommand
  {
    public int Id { get; set; }
  }
}
