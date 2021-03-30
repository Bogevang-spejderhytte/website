using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.TenantCategories;
using Bogevang.Common.Utility;
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
      IIgnorePermissionCheckHandler
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly ITemplateProvider TemplateProvider;
    private readonly ITenantCategoryProvider TenantCategoryProvider;
    private readonly IMailDispatchService MailDispatchService;
    private readonly BookingSettings BookingSettings;
    private readonly ICurrentUserProvider CurrentUserProvider;


    public BookingRequestCommandHandler(
      IAdvancedContentRepository domainRepository,
      ITemplateProvider templateProvider,
      ITenantCategoryProvider tenantCategoryProvider,
      IMailDispatchService mailDispatchService,
      BookingSettings bookingSettings,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      TemplateProvider = templateProvider;
      TenantCategoryProvider = tenantCategoryProvider;
      MailDispatchService = mailDispatchService;
      BookingSettings = bookingSettings;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(BookingRequestCommand command, IExecutionContext executionContext)
    {
      var tenantCategory = await TenantCategoryProvider.GetTenantCategoryById(command.TenantCategoryId.Value);

      DateTime lastAllowedArrivalDate = DateTime.Now.AddMonths(tenantCategory.AllowedBookingFutureMonths);
      if (command.ArrivalDate.Value >= lastAllowedArrivalDate)
        throw new ValidationErrorException(new ValidationError($"Den valgte lejertype kan ikke reservere mere end {tenantCategory.AllowedBookingFutureMonths} måneder ud i fremtiden. Dvs. senest {lastAllowedArrivalDate.ToShortDateString()}.", nameof(command.ArrivalDate)));

      var booking = new BookingDataModel
      {
        ArrivalDate = DateTime.SpecifyKind(command.ArrivalDate.Value, DateTimeKind.Utc),
        DepartureDate = DateTime.SpecifyKind(command.DepartureDate.Value, DateTimeKind.Utc),
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

      var addCommand = new AddCustomEntityCommand
      {
        CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
        Model = booking,
        Title = booking.MakeTitle(),
        Publish = true,

      };

      await DomainRepository.WithElevatedPermissions().CustomEntities().AddAsync(addCommand);

      // FIXME: Error handler and transactions
      await SendConfirmationMail(booking);
    }


    private async Task SendConfirmationMail(BookingDataModel booking)
    {
      TemplateDataModel template = await TemplateProvider.GetTemplateByName("reservationskvittering");

      string confirmationMailText = TemplateProvider.MergeText(template.Text, booking);

      MailAddress to = new MailAddress(booking.ContactEMail, booking.ContactName);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = "Kvittering for forespørgsel på Bøgevang",
        HtmlBody = confirmationMailText
      };

      await MailDispatchService.DispatchAsync(message);
    }
  }
}
