using System.Threading.Tasks;
using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/send-booking-mail")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class SendBookingMailApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public SendBookingMailApiController(
      IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public Task<JsonResult> Post([FromBody] SendBookingMailCommand command)
    {
      return ApiResponseHelper.RunCommandAsync(command);
    }
  }
}
