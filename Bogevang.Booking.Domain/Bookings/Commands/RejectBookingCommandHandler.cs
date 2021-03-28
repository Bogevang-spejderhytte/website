using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class RejectBookingCommandHandler
    : ICommandHandler<RejectBookingCommand>,
      IIgnorePermissionCheckHandler
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly ICurrentUserProvider CurrentUserProvider;

    
    public RejectBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(RejectBookingCommand command, IExecutionContext executionContext)
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
    }
  }
}
