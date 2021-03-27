using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.TenantCategories;
using Cofoundry.Core.DependencyInjection;

namespace Bogevang.Booking.Domain
{
  public class BookingDependencyRegistration : IDependencyRegistration
  {
    public void Register(IContainerRegister container)
    {
      container.Register<IBookingProvider, BookingProvider>();
      container.Register<IBookingMailService, BookingMailService>();
      container.Register<ITenantCategoryProvider, TenantCategoryProvider>();
    }
  }
}
