using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Website.Api
{
  [Route("api/booking-request")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class BookingRequestApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public BookingRequestApiController(
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public Task<JsonResult> Post([FromBody] BookingRequestCommand command)
    {
      return ApiResponseHelper.RunCommandAsync(command);

      //using (var scope = DomainRepository.Transactions().CreateScope())
      //{
      //}
    }
  }
}