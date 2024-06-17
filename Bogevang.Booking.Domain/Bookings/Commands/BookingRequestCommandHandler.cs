using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Documents.Commands;
using Bogevang.Booking.Domain.TenantCategories;
using Bogevang.Common.Utility;
using Bogevang.SequenceGenerator.Domain;
using Bogevang.Templates.Domain;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Core.Mail;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class BookingRequestCommandHandler
    : ICommandHandler<BookingRequestCommand>, 
      IIgnorePermissionCheckHandler // Anyone can add a booking request
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly ISequenceNumberGenerator SequenceNumberGenerator;
    private readonly ITemplateProvider TemplateProvider;
    private readonly ITenantCategoryProvider TenantCategoryProvider;
    private readonly IMailDispatchService MailDispatchService;
    private readonly ICommandExecutor CommandExecutor;
    private readonly BookingSettings BookingSettings;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public BookingRequestCommandHandler(
      IAdvancedContentRepository domainRepository,
      ISequenceNumberGenerator sequenceNumberGenerator,
      ITemplateProvider templateProvider,
      ITenantCategoryProvider tenantCategoryProvider,
      IMailDispatchService mailDispatchService,
      ICommandExecutor commandExecutor,
      BookingSettings bookingSettings,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      SequenceNumberGenerator = sequenceNumberGenerator;
      TemplateProvider = templateProvider;
      TenantCategoryProvider = tenantCategoryProvider;
      MailDispatchService = mailDispatchService;
      CommandExecutor = commandExecutor;
      BookingSettings = bookingSettings;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(BookingRequestCommand command, IExecutionContext executionContext)
    {
      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var tenantCategory = await TenantCategoryProvider.GetTenantCategoryById(command.TenantCategoryId.Value);

        DateTime lastAllowedArrivalDate = DateTime.Now.AddMonths(tenantCategory.AllowedBookingFutureMonths);
        if (command.ArrivalDate.Value >= lastAllowedArrivalDate || command.DepartureDate.Value >= lastAllowedArrivalDate)
          throw new ValidationErrorException(new ValidationError($"Den valgte lejertype kan ikke reservere mere end {tenantCategory.AllowedBookingFutureMonths} måneder ud i fremtiden. Dvs. senest {lastAllowedArrivalDate.ToShortDateString()}.", nameof(command.ArrivalDate)));

        int bookingNumber = await SequenceNumberGenerator.NextNumber("BookingNumber");

        var booking = new BookingDataModel
        {
          BookingNumber = bookingNumber,
          ArrivalDate = command.ArrivalDate.Value,
          DepartureDate = command.DepartureDate.Value,
          OnlySelectedWeekdays = command.OnlySelectedWeekdays,
          SelectedWeekdays = command.SelectedWeekdays,
          Location = command.Location,
          TenantCategoryId = command.TenantCategoryId.Value,
          TenantName = command.TenantName,
          Purpose = command.Purpose,
          ContactName = command.ContactName,
          ContactPhone = command.ContactPhone,
          ContactAddress = command.ContactAddress,
          ContactCity = command.ContactCity,
          ContactEMail = command.ContactEMail,
          Comments = command.Comments,
          RentalPrice = null, // To be set later
          Deposit = BookingSettings.StandardDeposit,
          BookingState = BookingDataModel.BookingStateType.Requested
        };

        await booking.AddLogEntry(CurrentUserProvider, "Reservationen blev indsendt af lejer.");

        await SendConfirmationMail(booking);
        await SendNotificationMail(booking);

        var addCommand = new AddCustomEntityCommand
        {
          CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
          Model = booking,
          Title = booking.MakeTitle(),
          Publish = true,

        };

        await DomainRepository.WithElevatedPermissions().CustomEntities().AddAsync(addCommand);

        await scope.CompleteAsync();
      }
    }


    private async Task SendConfirmationMail(BookingDataModel booking)
    {
      TemplateDataModel template = await TemplateProvider.GetTemplateByName("reservationskvittering");

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


    private async Task SendNotificationMail(BookingDataModel booking)
    {
      TemplateDataModel template = await TemplateProvider.GetTemplateByName("reservationsnotifikation");

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
