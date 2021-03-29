using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Commands;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Templates.Domain;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
