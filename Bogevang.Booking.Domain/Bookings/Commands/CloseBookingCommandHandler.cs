﻿using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class CloseBookingCommandHandler
    : ICommandHandler<CloseBookingCommand>,
      IIgnorePermissionCheckHandler
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public CloseBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(CloseBookingCommand command, IExecutionContext executionContext)
    {
      var booking = await BookingProvider.GetBookingById(command.Id);

      booking.BookingState = BookingDataModel.BookingStateType.Closed;

      await booking.AddLogEntry(CurrentUserProvider, "Reservationen blev afsluttet.");

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