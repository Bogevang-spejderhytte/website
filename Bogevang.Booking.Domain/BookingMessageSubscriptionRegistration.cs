using Bogevang.Booking.Domain.Bookings;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain;

namespace Bogevang.Booking.Domain
{
  public class BookingMessageSubscriptionRegistration : IMessageSubscriptionRegistration
  {
    public void Register(IMessageSubscriptionConfig config)
    {
      config.Subscribe<CustomEntityAddedMessage, BookingProvider>();
      config.Subscribe<ICustomEntityContentUpdatedMessage, BookingProvider>();
      config.Subscribe<CustomEntityDeletedMessage, BookingProvider>();
    }
  }
}
