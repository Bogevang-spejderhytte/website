using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/bookings")]
  [AutoValidateAntiforgeryToken]
  public class BookingsApiController : ControllerBase
  {
    private readonly IBookingProvider BookingProvider;
    private readonly IApiResponseHelper ApiResponseHelper;


    public BookingsApiController(
        IBookingProvider bookingProvider,
        IApiResponseHelper apiResponseHelper
        )
    {
      BookingProvider = bookingProvider;
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpGet("")]
    //[AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
    public async Task<JsonResult> Get([FromQuery] SearchBookingSummariesQuery query)
    {
      if (query == null) 
        query = new SearchBookingSummariesQuery();

      IEnumerable<BookingSummary> bookings = await BookingProvider.FindBookingsInInterval(query);

      return ApiResponseHelper.SimpleQueryResponse(bookings);
    }
  }
}
