using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Bogevang.Templates.Domain;

namespace Bogevang.StatusMail.Domain
{
  public class StatusMailProvider : IStatusMailProvider
  {
    protected IBookingProvider BookingProvider { get; set; }
    protected ITemplateProvider TemplateProvider { get; set; }
    protected IEloverblikAPI EloverblikAPI { get; set; }


    public StatusMailProvider(
      IBookingProvider bookingProvider,
      ITemplateProvider templateProvider,
      IEloverblikAPI eloverblikAPI)
    {
      BookingProvider = bookingProvider;
      TemplateProvider = templateProvider;
      EloverblikAPI = eloverblikAPI;
    }


    public async Task<string> BuildStatusMessage()
    {
      IDictionary<string, object> content = await BuildStatusContent();

      string template = TemplateProvider.GetEmbeddedTemplateByName(this.GetType().Assembly, "statusContent.html");

      string text = TemplateProvider.MergeText(template, content);

      return text;
    }


    public async Task<IDictionary<string, object>> BuildStatusContent()
    {
      SearchBookingSummariesQuery query = new SearchBookingSummariesQuery
      {
        Start = DateTime.Now.AddYears(-2),
        End = DateTime.Now.AddDays(14),
        BookingState = new BookingDataModel.BookingStateType[] { BookingDataModel.BookingStateType.Requested, BookingDataModel.BookingStateType.Approved }
      };

      var bookings = await BookingProvider.FindBookingsInInterval(query);
      var expandedBookingDays = bookings.SelectMany(b => b.ExpandDays()).ToDictionary(
        b => b.Date.ToString("yyyy-MM-dd"),
        b => b.Booking);

      List<Dictionary<string, object>> dates = new List<Dictionary<string, object>>();

      var startDate = DateTime.Now.Date.AddDays(-30);
      var endDate = DateTime.Now.Date.AddDays(14);

      var today = DateTime.Now.ToString("yyyy-MM-dd");

      for (DateTime day = startDate; day < endDate; day = day.AddDays(1))
      {
        string dayString = day.ToString("yyyy-MM-dd");
        Dictionary<string, object> dayData = new Dictionary<string, object>();

        dayData["date"] = day;
        dayData["dateString"] = dayString;

        string rowColor = "transparent";
        string dateColor = "transparent";

        if (today == dayString)
        {
          rowColor = "#90EE90";
        }
        else if (day.DayOfWeek == DayOfWeek.Saturday)
        {
          dateColor = "#D3D3D3";
        }
        if (day.DayOfWeek == DayOfWeek.Sunday)
        {
          rowColor = "#D3D3D3";
        }

        dayData["rowColor"] = rowColor;
        dayData["dateColor"] = dateColor;

        if (expandedBookingDays.TryGetValue(dayString, out BookingSummary booking))
        {
          dayData["bookingNumber"] = booking.BookingNumber;
          dayData["bookingAlert"] = booking.AlertMessage;
          dayData["bookingPurpose"] = booking.Purpose;
          dayData["bookingContactName"] = booking.ContactName;
        }

        dates.Add(dayData);
      }

      await LoadElectricityTimeSeries(startDate, endDate, dates);

      Dictionary<string, object> result = new Dictionary<string, object>();

      result["dates"] = dates;

      return result;
    }


    protected async Task LoadElectricityTimeSeries(DateTime start, DateTime end, List<Dictionary<string, object>> dates)
    {
      EloverblikAPI.TimeSeries electricityUsage = await EloverblikAPI.LoadElectricityTimeSeries(start, end);

      if (electricityUsage != null)
      {
        IDictionary<string, EloverblikAPI.PeriodEntry> periodMapping = electricityUsage.Period.ToDictionary(
          entry => entry?.timeInterval?.start.ToString("yyyy-MM-dd"),
          entry => entry);

        foreach (var day in dates)
        {
          string date = (string)day["dateString"];

          if (periodMapping.TryGetValue(date, out EloverblikAPI.PeriodEntry usageEntry))
          {
            day["electricityUsage"] = usageEntry.Point[0].out_Quantity_quantity;
          }
        }
      }
    }
  }
}
