using Bogevang.Booking.Domain.Bookings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/calendar")]
  public class CalendarApiController : ControllerBase
  {
    private IBookingService BookingService;

    public CalendarApiController(IBookingService bookingService)
    {
      BookingService = bookingService;
    }


    public class CalendarEvent
    {
      public string start { get; set; }
      public string end { get; set; }
      public string title { get; set; }
    }

    
    [HttpGet]
    public async Task<JsonResult> Get([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
      var bookings = await BookingService.FindBookingsInInterval(start, end);
      var events = bookings.Select(b => new CalendarEvent
      {
        start = b.ArrivalDate.Value.ToString("yyyy-MM-dd"),
        end = b.DepartureDate.Value.AddDays(1).ToString("yyyy-MM-dd"),
        title = "Optaget"
      });

      return new JsonResult(events);
    }
  }
}
