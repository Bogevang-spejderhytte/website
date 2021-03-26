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
    private readonly IBookingProvider BookingProvider;
    private readonly ITemplateProvider TemplateProvider;
    private readonly IMailDispatchService MailDispatchService;
    private readonly ICurrentUserProvider CurrentUserProvider;

    public SendBookingMailCommandHandler(
      IBookingProvider bookingProvider,
      ITemplateProvider templateProvider,
      IMailDispatchService mailDispatchService,
      ICurrentUserProvider currentUserProvider)
    {
      BookingProvider = bookingProvider;
      TemplateProvider = templateProvider;
      MailDispatchService = mailDispatchService;
      CurrentUserProvider = currentUserProvider;
    }


    public async Task ExecuteAsync(SendBookingMailCommand command, IExecutionContext executionContext)
    {
      BookingDataModel booking = await BookingProvider.GetBookingById(command.BookingId);
      // FIXME: verify booking exists

      MailAddress to = new MailAddress(booking.ContactEMail, booking.ContactName);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = command.Subject,
        HtmlBody = command.Message
      };

      await MailDispatchService.DispatchAsync(message);

      var user = await CurrentUserProvider.GetAsync();
      booking.AddLogEntry(new BookingLogEntry
      {
        Text = "Kvitteringsmail blev udsendt.",
        Username = user.User.GetFullName(),
        UserId = user.User.UserId,
        Timestamp = DateTime.Now
      });
    }
  }
}
