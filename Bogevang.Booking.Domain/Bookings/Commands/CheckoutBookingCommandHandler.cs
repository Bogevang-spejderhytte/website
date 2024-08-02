using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Documents.Commands;
using Bogevang.Common.AdminSettings;
using Bogevang.Common.Utility;
using Bogevang.Templates.Domain;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class CheckoutBookingCommandHandler
      : ICommandHandler<CheckoutBookingCommand>,
        IIgnorePermissionCheckHandler  // Depends on token checking
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly ITemplateProvider TemplateProvider;
    private readonly IMailDispatchService MailDispatchService;
    private readonly ICommandExecutor CommandExecutor;
    private readonly BookingSettings BookingSettings;
    private readonly ICurrentUserProvider CurrentUserProvider;
    private readonly IAdminSettingsProvider AdminSettingsProvider;


    public CheckoutBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      ITemplateProvider templateProvider,
      IMailDispatchService mailDispatchService,
      ICommandExecutor commandExecutor,
      BookingSettings bookingSettings,
      ICurrentUserProvider currentUserProvider,
      IAdminSettingsProvider adminSettingsProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      TemplateProvider = templateProvider;
      MailDispatchService = mailDispatchService;
      CommandExecutor = commandExecutor;
      BookingSettings = bookingSettings;
      CurrentUserProvider = currentUserProvider;
      AdminSettingsProvider = adminSettingsProvider;
    }


    public async Task ExecuteAsync(CheckoutBookingCommand command, IExecutionContext executionContext)
    {
      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        BookingDataModel booking = await BookingProvider.GetBookingById(command.Id);

        if (command.Token != booking.TenantSelfServiceToken)
          throw new AuthenticationFailedException("Ugyldigt eller manglende adgangsnøgle");

        if (booking.BookingState == BookingDataModel.BookingStateType.Requested)
          throw new ValidationErrorException("Det er ikke muligt at indberette slutafregning, da reservationen endnu ikke er godkendt.");
        else if (booking.BookingState == BookingDataModel.BookingStateType.Closed)
          throw new ValidationErrorException("Det er ikke muligt at indberette slutafregning, da reservationen allerede er afsluttet.");

        booking.IsCheckedOut = true;
        booking.ElectricityReadingStart = command.StartReading;
        booking.ElectricityReadingEnd = command.EndReading;
        booking.ElectricityPriceUnit = await AdminSettingsProvider.GetDecimalSetting("Elpris"); ;

        if (!string.IsNullOrEmpty(command.Comments))
          booking.Comments += $"\n\n=== Kommentarer til slutregnskab [{DateTime.Now}] ===\n{command.Comments}";

        await booking.AddLogEntry(CurrentUserProvider, "Elforbrug blev indmeldt af lejer.");

        // Do not use BookingSummary for mails as it will be the old version of the booking, from before readings were updated.
        // Also make sure mails are sent before updating the entity, as sent mail will be refered by the model.
        await SendCheckoutConfirmationMail(booking);
        await SendAdminNotificationMail(booking, !string.IsNullOrWhiteSpace(command.Comments));

        UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
        {
          CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
          CustomEntityId = command.Id,
          Title = booking.MakeTitle(),
          Publish = true,
          Model = booking
        };

        await DomainRepository.WithElevatedPermissions().CustomEntities().Versions().UpdateDraftAsync(updateCmd);

        await scope.CompleteAsync();
      }
    }


    private async Task SendCheckoutConfirmationMail(BookingDataModel booking)
    {
      TemplateDataModel template = await TemplateProvider.GetTemplateByName("slutafregningskvittering");

      string mailText = TemplateProvider.MergeText(template.Text, booking);

      MailAddress to = new MailAddress(booking.ContactEMail, booking.ContactName);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = template.Subject,
        HtmlBody = mailText
      };

      await MailDispatchService.DispatchAsync(message);

      byte[] mailBody = System.Text.Encoding.UTF8.GetBytes(message.HtmlBody);
      var addDcoumentCommand = new AddDocumentCommand
      {
        Title = message.Subject,
        MimeType = "text/html",
        Body = mailBody
      };

      await CommandExecutor.ExecuteAsync(addDcoumentCommand);
      booking.AddDocument(message.Subject, addDcoumentCommand.OutputDocumentId);
    }


    private async Task SendAdminNotificationMail(BookingDataModel booking, bool hasComment)
    {
      TemplateDataModel template = await TemplateProvider.GetTemplateByName("slutafregningsnotifikation");

      string mailText = TemplateProvider.MergeText(template.Text, booking, new { HasComment = hasComment });

      MailAddress to = new MailAddress(BookingSettings.AdminEmail);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = template.Subject,
        HtmlBody = mailText
      };

      await MailDispatchService.DispatchAsync(message);
    }
  }
}
