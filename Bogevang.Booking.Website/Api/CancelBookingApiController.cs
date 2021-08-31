using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/cancel-booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class CancelBookingApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public CancelBookingApiController(
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id)
    {
      var cancelCommand = new CancelBookingCommand { Id = id };
      return await ApiResponseHelper.RunCommandAsync(cancelCommand);
    }
  }
}
