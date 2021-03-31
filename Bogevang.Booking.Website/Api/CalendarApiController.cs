using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/calendar")]
  public class CalendarApiController : ControllerBase
  {
    private IBookingProvider BookingService;

    public CalendarApiController(IBookingProvider bookingService)
    {
      BookingService = bookingService;
    }


    public class CalendarEvent
    {
      public DateTime start { get; set; }
      public DateTime end { get; set; }
      public string title { get; set; }
      public bool allDay => true;
    }

    
    [HttpGet]
    public async Task<JsonResult> Get([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
      var bookings = await BookingService.FindBookingsInInterval(new SearchBookingSummariesQuery { Start = start, End = end });

      var expandedBookingDays = bookings.SelectMany(b => b.ExpandDays());

      var events = expandedBookingDays.Select(b => new CalendarEvent
      {
        start = b.Date,//.ToString("yyyy-MM-dd"),
        end = b.Date.AddDays(1),//.ToString("yyyy-MM-dd"),
        title = "Optaget"
      });

      return new JsonResult(events);
    }
  }
}
