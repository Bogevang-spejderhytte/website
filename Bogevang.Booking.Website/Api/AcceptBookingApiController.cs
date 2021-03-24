using Bogevang.Booking.Domain.Bookings.Commands;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/accept-booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class AcceptBookingApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public AcceptBookingApiController(
        IAdvancedContentRepository domainRepository,
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id)
    {
      var acceptCommand = new AcceptBookingCommand { Id = id };
      return await ApiResponseHelper.RunCommandAsync(acceptCommand);
    }
  }
}
