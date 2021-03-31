using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
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
    private readonly BookingSettings BookingSettings;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public CheckoutBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      ITemplateProvider templateProvider,
      IMailDispatchService mailDispatchService,
      BookingSettings bookingSettings,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      TemplateProvider = templateProvider;
      MailDispatchService = mailDispatchService;
      BookingSettings = bookingSettings;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(CheckoutBookingCommand command, IExecutionContext executionContext)
    {
      BookingDataModel booking = await BookingProvider.GetBookingById(command.Id);
      BookingSummary bookingSummary = await BookingProvider.GetBookingSummaryById(command.Id);

      if (command.Token != booking.TenantSelfServiceToken)
        throw new AuthenticationFailedException("Ugyldigt eller manglende adgangsnøgle");

      if (booking.BookingState == BookingDataModel.BookingStateType.Requested)
        throw new ValidationErrorException("Det er ikke muligt at indberette slutafregning, da reservationen endnu ikke er godkendt.");
      else if (booking.BookingState == BookingDataModel.BookingStateType.Closed)
        throw new ValidationErrorException("Det er ikke muligt at indberette slutafregning, da reservationen allerede er afsluttet.");

      booking.IsCheckedOut = true;
      booking.ElectricityReadingStart = command.StartReading;
      booking.ElectricityReadingEnd = command.EndReading;
      booking.ElectricityPriceUnit = BookingSettings.ElectricityPrice;

      if (!string.IsNullOrEmpty(command.Comments))
        booking.Comments += $"\n\n=== Kommentarer til slutregnskab [{DateTime.Now}] ===\n{command.Comments}";

      await booking.AddLogEntry(CurrentUserProvider, "Elforbrug blev indmeldt af lejer.");

      // FIXME: Error handler and transactions
      await SendCheckoutConfirmationMail(bookingSummary);

      await SendAdminNotificationMail(bookingSummary);

      UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
      {
        CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
        CustomEntityId = command.Id,
        Title = booking.MakeTitle(),
        Publish = true,
        Model = booking
      };

      await DomainRepository.WithElevatedPermissions().CustomEntities().Versions().UpdateDraftAsync(updateCmd);
    }


    private async Task SendCheckoutConfirmationMail(BookingSummary booking)
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
    }


    private async Task SendAdminNotificationMail(BookingSummary booking)
    {
      TemplateDataModel template = await TemplateProvider.GetTemplateByName("slutafregningsnotifikation");

      string mailText = TemplateProvider.MergeText(template.Text, booking);

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
