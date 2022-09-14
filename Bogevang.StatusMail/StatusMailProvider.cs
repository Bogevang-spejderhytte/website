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


    public async Task<string> BuildStatusCalendar(DateTime startDate, DateTime endDate)
    {
      try
      {
        IDictionary<string, object> content = await BuildStatusCalendarContent(startDate, endDate);

        string template = TemplateProvider.GetEmbeddedTemplateByName(this.GetType().Assembly, "statusContent.html");

        string text = TemplateProvider.MergeText(template, content);

        return text;
      }
      catch (Exception ex)
      {
        return $"Failed to create status calendar: {ex.Message}";
      }
    }


    public async Task<IDictionary<string, object>> BuildStatusCalendarContent(DateTime startDate, DateTime endDate)
    {
      SearchBookingSummariesQuery query = new SearchBookingSummariesQuery
      {
        Start = startDate,
        End = endDate
      };

      var bookings = await BookingProvider.FindBookingsInInterval(query);

      var expandedBookingDays = bookings
        .SelectMany(b => b.ExpandDays())
        .GroupBy(b => b.Date.ToString("yyyy-MM-dd"))
        .ToDictionary(
          b => b.Key,
          days => days.OrderBy(d => d.Booking.ArrivalDate).Select(d => d.Booking).ToList());

      List<Dictionary<string, object>> dates = new List<Dictionary<string, object>>();

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

        if (expandedBookingDays.TryGetValue(dayString, out List<BookingSummary> bday))
        {
          dayData["bookingNumber"] = string.Join("<br/>", bday.Select(b => b.BookingNumber));
          dayData["bookingAlert"] = bday.First().AlertMessage;
          dayData["bookingDetails"] = string.Join("<br/>", bday.Select(b => b.Purpose + "<br/>v/" + b.ContactName));
          dayData["bookingPurpose"] = bday.First().Purpose;
          dayData["bookingContactName"] = bday.First().ContactName;
        }

        dates.Add(dayData);
      }

      Dictionary<string, object> result = new Dictionary<string, object>();

      try
      {
        await LoadElectricityTimeSeries(startDate, endDate, dates);
      }
      catch (Exception ex)
      {
        result["message"] = ex.Message;
      }

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
