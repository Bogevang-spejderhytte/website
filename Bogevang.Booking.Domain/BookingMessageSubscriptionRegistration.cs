using Bogevang.Booking.Domain.Bookings;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain;

namespace Bogevang.Booking.Domain
{
  public class BookingMessageSubscriptionRegistration : IMessageSubscriptionRegistration
  {
    public void Register(IMessageSubscriptionConfig config)
    {
      config.Subscribe<CustomEntityAddedMessage, BookingService>();
      config.Subscribe<ICustomEntityContentUpdatedMessage, BookingService>();
      config.Subscribe<CustomEntityDeletedMessage, BookingService>();
    }
  }
}
