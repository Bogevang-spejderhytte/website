﻿using Bogevang.Booking.Domain.Bookings.CustomEntities;
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
    private readonly IBookingProvider BookingProvider;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public ApproveBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(ApproveBookingCommand command, IExecutionContext executionContext)
    {
      var booking = await BookingProvider.GetBookingById(command.Id);

      booking.BookingState = BookingDataModel.BookingStateType.Approved;
      booking.IsApproved = true;
      booking.IsRejected = false;

      await booking.AddLogEntry(CurrentUserProvider, "Reservationen blev godkendt.");

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
