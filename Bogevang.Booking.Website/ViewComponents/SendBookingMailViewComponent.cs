using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Commands;
using Bogevang.Booking.Domain.Bookings.Models;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class SendBookingMailViewComponent : ViewComponent
  {
    private readonly IBookingMailService BookingMailService;


    public SendBookingMailViewComponent(
      IBookingMailService bookingMailService)
    {
      BookingMailService = bookingMailService;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
      if (Request.Query.TryGetValue("id", out var id_s) 
        && int.TryParse(id_s, out int id)
        && Request.Query.TryGetValue("template", out var template))
      {
        BookingMail mail = await BookingMailService.CreateBookingMail(id, template);
        return View(mail);
      }

      return View(new BookingMail());
    }
  }
}
