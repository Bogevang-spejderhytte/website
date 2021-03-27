using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.TenantCategories;
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
      
      config.Subscribe<CustomEntityAddedMessage, TenantCategoryProvider>();
      config.Subscribe<ICustomEntityContentUpdatedMessage, TenantCategoryProvider>();
      config.Subscribe<CustomEntityDeletedMessage, TenantCategoryProvider>();
    }
  }
}
