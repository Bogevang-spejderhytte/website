using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class ApproveBookingCommandHandler
      : ICommandHandler<ApproveBookingCommand>,
        IIgnorePermissionCheckHandler  // Depends on custom entity permission checking

  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public ApproveBookingCommandHandler(
        IAdvancedContentRepository domainRepository,
        ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(ApproveBookingCommand command, IExecutionContext executionContext)
    {
      var entity = await DomainRepository.CustomEntities().GetById(command.Id).AsDetails().ExecuteAsync();
      // FIXME Check for being a booking
      BookingDataModel booking = (BookingDataModel)entity.LatestVersion.Model;

      booking.BookingState = BookingDataModel.BookingStateType.Approved;
      booking.IsApproved = true;
      booking.IsRejected = false;

      var user = await CurrentUserProvider.GetAsync();
      booking.AddLogEntry(new BookingLogEntry
      {
        Text = "Reservationen blev godkendt.",
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
    }
  }
}
