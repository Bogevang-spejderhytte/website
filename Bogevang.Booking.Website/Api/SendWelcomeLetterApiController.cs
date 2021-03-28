using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/send-welcome-letter")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class SendWelcomeLetterApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public SendWelcomeLetterApiController(
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id)
    {
      var sendWelcomeLetterCommand = new SendWelcomeLetterCommand { Id = id };
      return await ApiResponseHelper.RunCommandAsync(sendWelcomeLetterCommand);
    }
  }
}
