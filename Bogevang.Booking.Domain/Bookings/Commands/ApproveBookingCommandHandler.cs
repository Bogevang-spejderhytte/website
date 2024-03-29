﻿using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class ApproveBookingCommandHandler :
    ICommandHandler<ApproveBookingCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly IPermissionValidationService PermissionValidationService;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public ApproveBookingCommandHandler(
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


    public async Task ExecuteAsync(ApproveBookingCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var booking = await BookingProvider.GetBookingById(command.Id);

        booking.BookingState = BookingDataModel.BookingStateType.Approved;
        booking.IsApproved = true;
        booking.IsCancelled = false;

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

        await scope.CompleteAsync();
      }
    }
  }
}
