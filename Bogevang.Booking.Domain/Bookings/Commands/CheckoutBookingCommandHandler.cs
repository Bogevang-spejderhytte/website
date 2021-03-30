using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Bogevang.Templates.Domain;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class CheckoutBookingCommandHandler
      : ICommandHandler<CheckoutBookingCommand>,
        IIgnorePermissionCheckHandler  // Depends on custom entity permission checking

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
      var booking = await BookingProvider.GetBookingById(command.Id);

      if (command.Token != booking.TenantSelfServiceToken)
        throw new AuthenticationFailedException("Ugyldigt eller manglende adgangsnøgle");

      booking.IsCheckedOut = true;
      booking.ElectricityReadingStart = command.StartReading;
      booking.ElectricityReadingEnd = command.EndReading;

      if (!string.IsNullOrEmpty(command.Comments))
        booking.Comments += $"\n\n=== Kommentarer til slutregnskab [{DateTime.Now}] ===\n{command.Comments}";

      await booking.AddLogEntry(CurrentUserProvider, "Elforbrug blev indmeldt af lejer.");

      // FIXME: Do more stuff here
      // - Send summary letter to tenant
      // - Send notification to admin

      UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
      {
        CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
        CustomEntityId = command.Id,
        Title = booking.MakeTitle(),
        Publish = true,
        Model = booking
      };

      await DomainRepository.CustomEntities().Versions().UpdateDraftAsync(updateCmd);

      // FIXME: Error handler and transactions
      await SendCheckoutConfirmationMail(booking);
    }


    private async Task SendCheckoutConfirmationMail(BookingDataModel booking)
    {
      TemplateDataModel template = await TemplateProvider.GetTemplateByName("slutafregningskvittering");

      string confirmationMailText = TemplateProvider.MergeText(template.Text, booking);

      MailAddress to = new MailAddress(booking.ContactEMail, booking.ContactName);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = "Kvittering for slutafregning på Bøgevang",
        HtmlBody = confirmationMailText
      };

      await MailDispatchService.DispatchAsync(message);
    }
  }
}
