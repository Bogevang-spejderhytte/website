using Microsoft.AspNetCore.Mvc;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingListViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View();
    }
  }
}
