using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Templates.Domain;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Core.Mail;
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
    private readonly IMailDispatchService MailDispatchService;


    public BookingRequestCommandHandler(
        IAdvancedContentRepository domainRepository,
        ITemplateProvider templateProvider,
        IMailDispatchService mailDispatchService)
    {
      DomainRepository = domainRepository;
      TemplateProvider = templateProvider;
      MailDispatchService = mailDispatchService;
    }


    public async Task ExecuteAsync(BookingRequestCommand command, IExecutionContext executionContext)
    {
      decimal rentalPrice = 1000;

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
        RentalPrice = rentalPrice,
        BookingState = BookingDataModel.BookingStateType.Requested
      };

      var addCommand = new AddCustomEntityCommand
      {
        CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
        Model = booking,
        Title = "Reservation",
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
