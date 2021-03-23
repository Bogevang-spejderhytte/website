using Bogevang.Booking.Domain.Bookings;
using Cofoundry.Core.DependencyInjection;

namespace Bogevang.Booking.Domain
{
  public class BookingDependencyRegistration : IDependencyRegistration
  {
    public void Register(IContainerRegister container)
    {
      container.Register<IBookingProvider, BookingProvider>();
    }
  }
}
