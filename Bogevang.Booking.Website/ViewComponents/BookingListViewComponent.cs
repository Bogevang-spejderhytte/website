using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingListViewComponent : ViewComponent
  {
    private readonly IContentRepository ContentRepository;

    public BookingListViewComponent(IContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }


    public IViewComponentResult Invoke()
    {
      return View();
    }
  }
}
