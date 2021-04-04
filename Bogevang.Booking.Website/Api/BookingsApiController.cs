using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain.CQS;
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
    private readonly IQueryExecutor QueryExecutor;
    private readonly IApiResponseHelper ApiResponseHelper;


    public BookingsApiController(
      IQueryExecutor queryExecutor,
      IApiResponseHelper apiResponseHelper)
    {
      QueryExecutor = queryExecutor;
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost("")]
    public async Task<JsonResult> Get([FromBody] SearchBookingSummariesQuery query)
    {
      IList<BookingSummary> bookings = await QueryExecutor.ExecuteAsync(query);
      return ApiResponseHelper.SimpleQueryResponse(bookings);
    }
  }
}
