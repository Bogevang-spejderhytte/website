using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/checkout-booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class CheckoutBookingApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public CheckoutBookingApiController(
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id, [FromBody] CheckoutBookingCommand command)
    {
      command.Id = id;
      return await ApiResponseHelper.RunCommandAsync(command);
    }
  }
}
