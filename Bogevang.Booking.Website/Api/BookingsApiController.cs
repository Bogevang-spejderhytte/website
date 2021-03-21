using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/bookings")]
  [AutoValidateAntiforgeryToken]
  public class BookingsApiController : ControllerBase
  {
    private readonly IDomainRepository DomainRepository;
    private readonly IApiResponseHelper ApiResponseHelper;


    public BookingsApiController(
        IDomainRepository domainRepository,
        IApiResponseHelper apiResponseHelper
        )
    {
      DomainRepository = domainRepository;
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpGet("")]
    //[AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
    public async Task<JsonResult> Get([FromQuery] SearchBookingSummariesQuery query)
    {
      if (query == null) 
        query = new SearchBookingSummariesQuery();

      PagedQueryResult<BookingSummary> results = await DomainRepository.ExecuteQueryAsync(query);

      return ApiResponseHelper.SimpleQueryResponse(results);
    }
  }
}
