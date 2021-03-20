using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
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
  public class BookingApiController : ControllerBase
  {
    private readonly IAdvancedContentRepository _domainRepository;
    private readonly IApiResponseHelper _apiResponseHelper;


    public BookingApiController(
        IAdvancedContentRepository domainRepository,
        IApiResponseHelper apiResponseHelper
        )
    {
      _domainRepository = domainRepository;
      _apiResponseHelper = apiResponseHelper;
    }


    [HttpGet]
    //[AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
    public async Task<JsonResult> Get([FromQuery] int id)
    {
      var entity = await _domainRepository.CustomEntities().GetById(id).AsDetails().ExecuteAsync();

      // FIXME: check for custom entity type being a "Booking"

      BookingDataModel booking = (BookingDataModel)entity.LatestVersion.Model;

      return _apiResponseHelper.SimpleQueryResponse(booking);
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
    public async Task<JsonResult> Post([FromQuery] int id)
    {
      var x = 10;

      return await Task.FromResult(new JsonResult(new { ok = true }));
    }
  }
}