using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Queries;
using Bogevang.Booking.Domain.Documents.Commands;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.CQS.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class AnonymizeBookingsCommandHandler :
    ICommandHandler<AnonymizeBookingsCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IBookingProvider BookingProvider;
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly ICommandExecutor CommandExecutor;
    private readonly IPermissionValidationService PermissionValidationService;


    public AnonymizeBookingsCommandHandler(
      IBookingProvider bookingProvider,
      IAdvancedContentRepository domainRepository,
      ICommandExecutor executor,
      IPermissionValidationService permissionValidationService)
    {
      BookingProvider = bookingProvider;
      DomainRepository = domainRepository;
      CommandExecutor = executor;
      PermissionValidationService = permissionValidationService;
    }


    public async Task ExecuteAsync(AnonymizeBookingsCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityDeletePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      var query = new SearchBookingSummariesQuery
      {
        BookingState = new BookingDataModel.BookingStateType[] { BookingDataModel.BookingStateType.Closed },
        Start = new DateTime(2000,1,1),
        End = DateTime.Now.AddYears(-3)
      };

      command.AnonymizedCount = 0;

      foreach (KeyValuePair<int, BookingDataModel> bookingEntry in (await BookingProvider.FindBookingDataInInterval(query)).ToList())
      {
        BookingDataModel booking = bookingEntry.Value;

        // Protected agains mistakes in the query by checking values again
        if (!booking.IsArchived
          && booking.BookingState == BookingDataModel.BookingStateType.Closed 
          && booking.DepartureDate.Value.AddYears(3) < DateTime.Now)
        {
          booking.TenantName = "---";
          booking.Purpose = "---";
          booking.ContactName = "---";
          booking.ContactEMail = "ukendt@ukendte-mailmodtagere";
          booking.ContactPhone = "---";
          booking.ContactAddress = "---";
          booking.ContactCity = "---";
          booking.Comments = "---";

          foreach (var document in booking.Documents)
          {
            var deleteDocumentCommand = new DeleteDocumentCommand { Id = document.DocumentId };
            await CommandExecutor.ExecuteAsync(deleteDocumentCommand);
          }

          booking.LogEntries.Clear();
          booking.Documents.Clear();

          booking.IsArchived = true;
          command.AnonymizedCount++;

          UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
          {
            CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
            CustomEntityId = bookingEntry.Key,
            Title = booking.MakeTitle(),
            Publish = true,
            Model = booking
          };

          await DomainRepository.CustomEntities().Versions().UpdateDraftAsync(updateCmd);
        }
      }
    }
  }
}
