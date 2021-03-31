using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/close-booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class CloseBookingApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public CloseBookingApiController(
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id)
    {
      var closeCommand = new CloseBookingCommand { Id = id };
      return await ApiResponseHelper.RunCommandAsync(closeCommand);
    }
  }
}
