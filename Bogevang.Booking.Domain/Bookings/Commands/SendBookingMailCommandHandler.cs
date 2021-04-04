using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Documents.Commands;
using Bogevang.Common.Utility;
using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class SendBookingMailCommandHandler : 
    ICommandHandler<SendBookingMailCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly IMailDispatchService MailDispatchService;
    private readonly ICommandExecutor CommandExecutor;
    private readonly IPermissionValidationService PermissionValidationService;
    private readonly ICurrentUserProvider CurrentUserProvider;

    public SendBookingMailCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      IMailDispatchService mailDispatchService,
      ICommandExecutor commandExecutor,
      IPermissionValidationService permissionValidationService,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      MailDispatchService = mailDispatchService;
      CommandExecutor = commandExecutor;
      PermissionValidationService = permissionValidationService;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(SendBookingMailCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        BookingDataModel booking = await BookingProvider.GetBookingById(command.BookingId);

        await booking.AddLogEntry(CurrentUserProvider, $"Sendt: {command.Subject}.");

        byte[] mailBody = System.Text.Encoding.UTF8.GetBytes(command.Message);
        var addDcoumentCommand = new AddDocumentCommand
        {
          Title = command.Subject,
          MimeType = "text/html",
          Body = mailBody
        };

        await CommandExecutor.ExecuteAsync(addDcoumentCommand);
        booking.AddDocument(command.Subject, addDcoumentCommand.OutputDocumentId);

        UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
        {
          CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
          CustomEntityId = command.BookingId,
          Title = booking.MakeTitle(),
          Publish = true,
          Model = booking
        };

        await DomainRepository.CustomEntities().Versions().UpdateDraftAsync(updateCmd);

        MailAddress to = new MailAddress(booking.ContactEMail, booking.ContactName);
        MailMessage message = new MailMessage
        {
          To = to,
          Subject = command.Subject,
          HtmlBody = command.Message
        };

        // It is not really a good idea to contact a mail server during a transaction, but ...
        // 1) I really don't want the mail being registered in the database as "sent" if the mail sending fails.
        // 2) One can hope that the dispatcher simply adds the message to an outgoing mail queue.
        // (and, yes, sending mails may fail much later with an "unknown recipient" or similar)

        await MailDispatchService.DispatchAsync(message);

        await scope.CompleteAsync();
      }
    }
  }
}
