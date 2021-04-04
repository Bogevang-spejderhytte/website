using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.TenantCategories;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class UpdateBookingCommandHandler :
    ICommandHandler<UpdateBookingCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly ITenantCategoryProvider TenantCategoryProvider;
    private readonly IPermissionValidationService PermissionValidationService;


    public UpdateBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      IPermissionValidationService permissionValidationService,
      ITenantCategoryProvider tenantCategoryProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      PermissionValidationService = permissionValidationService;
      TenantCategoryProvider = tenantCategoryProvider;
    }


    public async Task ExecuteAsync(UpdateBookingCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        // Verify teneant category
        await TenantCategoryProvider.GetTenantCategoryById(command.TenantCategoryId.Value);

        BookingDataModel booking = await BookingProvider.GetBookingById(command.BookingId);

        booking.ArrivalDate = command.ArrivalDate.Value;
        booking.DepartureDate = command.DepartureDate.Value;
        booking.OnlySelectedWeekdays = command.OnlySelectedWeekdays;
        booking.SelectedWeekdays = command.SelectedWeekdays;
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
        booking.Deposit = command.Deposit;
        booking.DepositReceived = command.DepositReceived;
        booking.ElectricityReadingStart = command.ElectricityReadingStart;
        booking.ElectricityReadingEnd = command.ElectricityReadingEnd;
        booking.ElectricityPriceUnit = command.ElectricityPriceUnit;

        UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
        {
          CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
          CustomEntityId = command.BookingId,
          Title = booking.MakeTitle(),
          Publish = true,
          Model = booking
        };

        await DomainRepository.CustomEntities().Versions().UpdateDraftAsync(updateCmd);

        await scope.CompleteAsync();
      }
    }
  }
}
