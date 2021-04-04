using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class SendWelcomeLetterCommandHandler :
    ICommandHandler<SendWelcomeLetterCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly IPermissionValidationService PermissionValidationService;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public SendWelcomeLetterCommandHandler(
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


    public async Task ExecuteAsync(SendWelcomeLetterCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var booking = await BookingProvider.GetBookingById(command.Id);

        booking.WelcomeLetterIsSent = true;

        // Welcome is not really sent here, we only redirect to the mail editing-and-sending page afterwards.

        var user = await CurrentUserProvider.GetAsync();
        booking.AddLogEntry(new BookingLogEntry
        {
          Text = "Velkomstbrev er udsendt.",
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
