using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Microsoft.AspNetCore.Mvc;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingEditorViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View(new BookingDataModel());
    }
  }
}
