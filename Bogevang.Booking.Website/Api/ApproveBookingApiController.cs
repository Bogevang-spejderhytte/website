﻿using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/approve-booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class ApproveBookingApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public ApproveBookingApiController(
        IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id)
    {
      var approveCommand = new ApproveBookingCommand { Id = id };
      return await ApiResponseHelper.RunCommandAsync(approveCommand);
    }
  }
}
