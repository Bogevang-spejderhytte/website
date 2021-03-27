using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class UpdateBookingCommandHandler
    : ICommandHandler<UpdateBookingCommand>,
      IIgnorePermissionCheckHandler // FIXME
  {
    private readonly IAdvancedContentRepository DomainRepository;


    public UpdateBookingCommandHandler(
        IAdvancedContentRepository domainRepository)
    {
      DomainRepository = domainRepository;
    }


    public async Task ExecuteAsync(UpdateBookingCommand command, IExecutionContext executionContext)
    {
      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var entity = await DomainRepository.CustomEntities().GetById(command.BookingId).AsDetails().ExecuteAsync();
        // FIXME: check for custom entity type being a "Booking"
        BookingDataModel booking = (BookingDataModel)entity.LatestVersion.Model;

        booking.ArrivalDate = command.ArrivalDate.Value;
        booking.DepartureDate = command.DepartureDate.Value;
        booking.TenantCategoryId = command.TenantCategoryId;
        booking.TenantName = command.TenantName;
        booking.Purpose = command.Purpose;
        booking.ContactName = command.ContactName;
        booking.ContactPhone = command.ContactPhone;
        booking.ContactAddress = command.ContactAddress;
        booking.ContactCity = command.ContactCity;
        booking.ContactEMail = command.ContactEMail;
        booking.Comments = command.Comments;
        booking.RentalPrice = command.RentalPrice;
        booking.BookingState = command.BookingState;

        UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
        {
          CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
          CustomEntityId = command.BookingId,
          Title = entity.LatestVersion.Title,
          Publish = true,
          Model = booking
        };

        await DomainRepository.CustomEntities().Versions().UpdateDraftAsync(updateCmd);

        await scope.CompleteAsync();
      }
    }
  }
}
