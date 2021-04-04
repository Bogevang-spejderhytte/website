using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Commands;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class BookingApiController : ControllerBase
  {
    private readonly IBookingProvider BookingProvider;
    private readonly IApiResponseHelper ApiResponseHelper;


    public BookingApiController(
        IBookingProvider bBookingProvider,
        IApiResponseHelper apiResponseHelper)
    {
      BookingProvider = bBookingProvider;
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpGet]
    public async Task<JsonResult> Get([FromQuery] int id)
    {
      var booking = await BookingProvider.GetBookingSummaryById(id);
      return ApiResponseHelper.SimpleQueryResponse(booking);
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id, [FromBody] UpdateBookingCommand command)
    {
      command.BookingId = id;
      return await ApiResponseHelper.RunCommandAsync(command);
    }


    [HttpDelete]
    public async Task<JsonResult> Delete([FromQuery] int id)
    {
      var deleteCmd = new DeleteBookingCommand { Id = id };
      return await ApiResponseHelper.RunCommandAsync(deleteCmd);
    }
 }
}