using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class RejectBookingCommandHandler :
    ICommandHandler<RejectBookingCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly IPermissionValidationService PermissionValidationService;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public RejectBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      IPermissionValidationService permissionValidationService,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      PermissionValidationService = permissionValidationService;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(RejectBookingCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var booking = await BookingProvider.GetBookingById(command.Id);

        booking.BookingState = BookingDataModel.BookingStateType.Closed;
        booking.IsApproved = false;
        booking.IsRejected = true;

        var user = await CurrentUserProvider.GetAsync();
        booking.AddLogEntry(new BookingLogEntry
        {
          Text = "Reservationen blev afvist.",
          Username = user.User.GetFullName(),
          UserId = user.User.UserId,
          Timestamp = DateTime.Now
        });

        UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
        {
          CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
          CustomEntityId = command.Id,
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
