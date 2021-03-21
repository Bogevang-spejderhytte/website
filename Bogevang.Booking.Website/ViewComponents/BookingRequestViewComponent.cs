using Bogevang.Booking.Domain.Bookings.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingRequestViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View(new BookingRequestCommand());
    }
  }
}
