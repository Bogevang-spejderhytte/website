using System;
using System.Linq;
using System.Threading.Tasks;
using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/calendar")]
  public class CalendarApiController : ControllerBase
  {
    private readonly IBookingProvider BookingService;
    private readonly IPermissionValidationService PermissionValidationService;
    private readonly IExecutionContextFactory ExecutionContextFactory;

    public CalendarApiController(
      IBookingProvider bookingService,
      IPermissionValidationService permissionValidationService,
      IExecutionContextFactory executionContextFactory)
    {
      BookingService = bookingService;
      PermissionValidationService = permissionValidationService;
      ExecutionContextFactory = executionContextFactory;
    }


    public class CalendarEvent
    {
      public DateTime start { get; set; }
      public string title { get; set; }
      public string description { get; set; }
      public bool allDay => true;
      public int? booking_id { get; set; }
    }

    
    [HttpGet]
    public async Task<JsonResult> Get([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
      var bookings = await BookingService.FindBookingsInInterval(new SearchBookingSummariesQuery 
      { 
          Start = start, 
          End = end,
          IsCancelled = false
      });

      var expandedBookingDays = bookings.SelectMany(b => b.ExpandDays());

      IExecutionContext executionContext = await ExecutionContextFactory.CreateAsync();
      bool hasEditAccess = PermissionValidationService.HasCustomEntityPermission<CustomEntityUpdatePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);
      
      var events = expandedBookingDays.Select(b => new CalendarEvent
      {
        start = b.Date,
        title = "Optaget",
        description = hasEditAccess ? b.Booking.Purpose : null,
        booking_id = hasEditAccess ? b.Booking.Id : (int?)null
      });

      return new JsonResult(events);
    }
  }
}
