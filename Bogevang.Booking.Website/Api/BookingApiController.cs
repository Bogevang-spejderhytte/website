using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Commands;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
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
    private readonly IPermissionValidationService PermissionValidationService;
    private readonly IExecutionContextFactory ExecutionContextFactory;
    private readonly IApiResponseHelper ApiResponseHelper;


    public BookingApiController(
      IBookingProvider bookingProvider,
      IPermissionValidationService permissionValidationService,
      IExecutionContextFactory executionContextFactory,
      IApiResponseHelper apiResponseHelper)
    {
      BookingProvider = bookingProvider;
      PermissionValidationService = permissionValidationService;
      ExecutionContextFactory = executionContextFactory;
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpGet]
    public async Task<object> Get([FromQuery] int id)
    {
      IExecutionContext executionContext = await ExecutionContextFactory.CreateAsync();
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      BookingSummary booking = await BookingProvider.GetBookingSummaryById(id);
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