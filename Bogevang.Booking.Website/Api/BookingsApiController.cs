using Bogevang.Booking.Domain.Models;
using Bogevang.Booking.Domain.Queries;
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
    private readonly IDomainRepository _domainRepository;
    private readonly IApiResponseHelper _apiResponseHelper;


    public BookingsApiController(
        IDomainRepository domainRepository,
        IApiResponseHelper apiResponseHelper
        )
    {
      _domainRepository = domainRepository;
      _apiResponseHelper = apiResponseHelper;
    }


    [HttpGet("")]
    //[AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
    public async Task<JsonResult> Get([FromQuery] SearchBookingSummariesQuery query)
    {
      if (query == null) 
        query = new SearchBookingSummariesQuery();

      PagedQueryResult<BookingSummary> results = await _domainRepository.ExecuteQueryAsync(query);

      return _apiResponseHelper.SimpleQueryResponse(results);
    }
  }
}
