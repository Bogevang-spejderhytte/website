using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Common.Utility;
using Bogevang.Templates.Domain;
using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class SendBookingMailCommandHandler : 
    ICommandHandler<SendBookingMailCommand>,
    IIgnorePermissionCheckHandler  // FIXME!!!!
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly IBookingProvider BookingProvider;
    private readonly ITemplateProvider TemplateProvider;
    private readonly IMailDispatchService MailDispatchService;
    private readonly ICurrentUserProvider CurrentUserProvider;

    public SendBookingMailCommandHandler(
      IAdvancedContentRepository domainRepository,
      IBookingProvider bookingProvider,
      ITemplateProvider templateProvider,
      IMailDispatchService mailDispatchService,
      ICurrentUserProvider currentUserProvider)
    {
      DomainRepository = domainRepository;
      BookingProvider = bookingProvider;
      TemplateProvider = templateProvider;
      MailDispatchService = mailDispatchService;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(SendBookingMailCommand command, IExecutionContext executionContext)
    {
      BookingDataModel booking = await BookingProvider.GetBookingById(command.BookingId);

      MailAddress to = new MailAddress(booking.ContactEMail, booking.ContactName);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = command.Subject,
        HtmlBody = command.Message
      };

      await MailDispatchService.DispatchAsync(message);

      await booking.AddLogEntry(CurrentUserProvider, $"Sendt: {command.Subject}.");

      UpdateCustomEntityDraftVersionCommand updateCmd = new UpdateCustomEntityDraftVersionCommand
      {
        CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
        CustomEntityId = command.BookingId,
        Title = booking.MakeTitle(),
        Publish = true,
        Model = booking
      };

      await DomainRepository.CustomEntities().Versions().UpdateDraftAsync(updateCmd);
    }
  }
}
