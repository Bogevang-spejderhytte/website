using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Api
{
  [Route("api/booking")]
  [AutoValidateAntiforgeryToken]
  [ApiController]
  public class BookingApiController : ControllerBase
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IApiResponseHelper ApiResponseHelper;
    private UpdateCustomEntityDraftVersionCommandHandler UpdateCustomEntityDraftVersionCommandHandler;


    public BookingApiController(
        IAdvancedContentRepository domainRepository,
        IApiResponseHelper apiResponseHelper,
        UpdateCustomEntityDraftVersionCommandHandler updateCustomEntityDraftVersionCommandHandler)
    {
      DomainRepository = domainRepository;
      ApiResponseHelper = apiResponseHelper;
      UpdateCustomEntityDraftVersionCommandHandler = updateCustomEntityDraftVersionCommandHandler;
    }


    [HttpGet]
    //[AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
    public async Task<JsonResult> Get([FromQuery] int id)
    {
      var entity = await DomainRepository.CustomEntities().GetById(id).AsDetails().ExecuteAsync();

      // FIXME: check for custom entity type being a "Booking"

      BookingDataModel booking = (BookingDataModel)entity.LatestVersion.Model;
      booking.ArrivalDate = booking.ArrivalDate.Value.ToLocalTime();
      booking.DepartureDate = booking.DepartureDate.Value.ToLocalTime();

      return ApiResponseHelper.SimpleQueryResponse(booking);
    }

    //private BookingDetails MapToBookingDetails(CustomEntityDetails ent)
    //{
    //  var b = (BookingDataModel)ent.LatestVersion.Model;

    //  return new BookingDetails
    //  {
    //    ArrivalDate = b.ArrivalDate?.ToString("yyyy-MM-dd"),
    //    DepartureDate = b.DepartureDate?.ToString("yyyy-MM-dd"),
    //    TenantCategoryId = b.TenantCategoryId,
    //    TenantName = b.TenantName,
    //    Purpose = b.Purpose,
    //    ContactName = b.ContactName,
    //    ContactEMail = b.ContactEMail,
    //    ContactPhone = b.ContactPhone,
    //    ContactAddress = b.ContactAddress,
    //    ContactCity = b.ContactCity,
    //    Comments = b.Comments,
    //    RentalPrice = b.RentalPrice,
    //    DepositReceived = b.DepositReceived,
    //    PaymentReceived = b.PaymentReceived,
    //    DepositReturned = b.DepositReturned
    //  };
    //}

    [HttpPost]
    //[AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
    public async Task<JsonResult> Post([FromQuery] int id, [FromBody]BookingDataModel input)
    {
      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var entity = await DomainRepository.CustomEntities().GetById(id).AsDetails().ExecuteAsync();
        // FIXME: check for custom entity type being a "Booking"
        BookingDataModel booking = (BookingDataModel)entity.LatestVersion.Model;

        booking.ArrivalDate = input.ArrivalDate.Value.ToUniversalTime();
        booking.DepartureDate = input.DepartureDate.Value.ToUniversalTime();
        booking.TenantCategoryId = input.TenantCategoryId;
        booking.TenantName = input.TenantName;
        booking.Purpose = input.Purpose;
        booking.ContactName = input.ContactName;
        booking.ContactPhone = input.ContactPhone;
        booking.ContactAddress = input.ContactAddress;
        booking.ContactCity = input.ContactCity;
        booking.ContactEMail = input.ContactEMail;
        booking.Comments = input.Comments;
        booking.RentalPrice = input.RentalPrice;
        booking.BookingState = input.BookingState;

        UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
        {
          CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
          CustomEntityId = id,
          Title = entity.LatestVersion.Title,
          Publish = true,
          Model = booking
        };

        await DomainRepository.CustomEntities().Versions().UpdateDraftAsync(updateCmd);

        await scope.CompleteAsync();

        return new JsonResult(new { ok = true });
      }
    }
  }
}