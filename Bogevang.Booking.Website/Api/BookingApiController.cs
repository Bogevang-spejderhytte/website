using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
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


    public BookingApiController(
        IAdvancedContentRepository domainRepository,
        IApiResponseHelper apiResponseHelper)
    {
      DomainRepository = domainRepository;
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpGet]
    public async Task<JsonResult> Get([FromQuery] int id)
    {
      var entity = await DomainRepository.CustomEntities().GetById(id).AsDetails().ExecuteAsync();
      // FIXME: check for custom entity type being a "Booking"
      BookingDataModel booking = (BookingDataModel)entity.LatestVersion.Model;

      return ApiResponseHelper.SimpleQueryResponse(booking);
    }


    [HttpPost]
    public async Task<JsonResult> Post([FromQuery] int id, [FromBody]BookingDataModel input)
    {
      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var entity = await DomainRepository.CustomEntities().GetById(id).AsDetails().ExecuteAsync();
        // FIXME: check for custom entity type being a "Booking"
        BookingDataModel booking = (BookingDataModel)entity.LatestVersion.Model;

        // For safety reasons, only pick what is needed from the posted data model. Do not assume anything else is ok.
        // (it is kind of cheating when using the data model as a command)
        booking.ArrivalDate = input.ArrivalDate;
        booking.DepartureDate = input.DepartureDate;
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