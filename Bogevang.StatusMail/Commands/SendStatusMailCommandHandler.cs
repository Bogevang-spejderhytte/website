using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Queries;
using Bogevang.Templates.Domain;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Bogevang.StatusMail.Domain.Commands
{
  public class SendStatusMailCommandHandler :
    ICommandHandler<SendStatusMailCommand>,
    IIgnorePermissionCheckHandler
  {
    protected readonly IStatusMailProvider StatusMailProvider;

    private readonly StatusMailSettings Settings;
    private readonly ITemplateProvider TemplateProvider;
    private readonly IMailDispatchService MailDispatchService;
    private readonly IBookingProvider BookingProvider;


    public SendStatusMailCommandHandler(
      IStatusMailProvider statusMailProvider,
      ITemplateProvider templateProvider,
      IMailDispatchService mailDispatchService,
      StatusMailSettings statusMailSettings,
      IBookingProvider bookingProvider)
    {
      StatusMailProvider = statusMailProvider;
      TemplateProvider = templateProvider;
      MailDispatchService = mailDispatchService;
      Settings = statusMailSettings;
      BookingProvider = bookingProvider;
    }


    public async Task ExecuteAsync(SendStatusMailCommand command, IExecutionContext executionContext)
    {
      DateTime startDate = DateTime.Now.AddDays(-14);
      DateTime endDate = DateTime.Now.AddDays(14);

      string calendarContent = await StatusMailProvider.BuildStatusCalendar(startDate, endDate);

      SearchBookingSummariesQuery query = new SearchBookingSummariesQuery
      {
        Start = new DateTime(DateTime.Now.Year, 1, 1),
        End = new DateTime(DateTime.Now.Year, 12, 31),
        //BookingState = new BookingDataModel.BookingStateType[] { BookingDataModel.BookingStateType.Requested, BookingDataModel.BookingStateType.Approved }
      };

      var bookings = (await BookingProvider.FindBookingsInInterval(query)).Where(b => !b.IsCancelled).ToList();

      int bookingCount = bookings.Count;
      decimal bookingTotalIncome = bookings.Sum(b => b.RentalPrice ?? 0.0M);

      TemplateDataModel template = await TemplateProvider.GetTemplateByName("statusmail");

      Dictionary<string, object> mergeData = new Dictionary<string, object>
      {
        ["date"] = DateTime.Now.Date,
        ["bookingCount"] = bookingCount,
        ["bookingTotalIncome"] = bookingTotalIncome,
        ["calendar"] = calendarContent
      };
      
      string mailContent = TemplateProvider.MergeText(template.Text, mergeData);

      MailAddress to = new MailAddress(Settings.MailReceiver);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = "Bøgevang statusopdatering",
        HtmlBody = mailContent
      };

      await MailDispatchService.DispatchAsync(message);
    }
  }
}
