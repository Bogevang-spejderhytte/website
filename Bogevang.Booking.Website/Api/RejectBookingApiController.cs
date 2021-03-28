using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/reject-booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class RejectBookingApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public RejectBookingApiController(
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id)
    {
      var rejectCommand = new RejectBookingCommand { Id = id };
      return await ApiResponseHelper.RunCommandAsync(rejectCommand);
    }
  }
}
