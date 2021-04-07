using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.TenantCategories;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Models
{
  public class BookingSummary
  {
    public enum AlertType
    {
      New,
      Key,
      AwaitingCheckout,
      Finalize
    }

    public enum NotificationLevelType
    {
      Information,
      Warning,
      Error
    }

    public class Notification
    {
      public NotificationLevelType Level { get; set; }
      public string Message { get; set; }
    }


    public int Id { get; set; }
    public int BookingNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public bool OnlySelectedWeekdays { get; set; }
    [CheckboxList(typeof(WeekdayType))]
    public ICollection<WeekdayType> SelectedWeekdays { get; set; }
    public int TenantCategoryId { get; set; }
    public string TenantName { get; set; }
    public string Purpose { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactAddress { get; set; }
    public string ContactCity { get; set; }
    public string ContactEMail { get; set; }
    public string Comments { get; set; }
    public decimal? RentalPrice { get; set; }
    public decimal? Deposit { get; set; }
    public bool DepositReceived { get; set; }
    public BookingDataModel.BookingStateType BookingState { get; set; }
    public string BookingStateText { get; set; }
    public bool IsApproved { get; set; }
    public bool IsRejected { get; set; }
    public bool WelcomeLetterIsSent { get; set; }
    public bool IsCheckedOut { get; set; }
    public bool IsArchived { get; set; }
    public string TenantSelfServiceToken { get; set; }
    public string CheckoutUrl { get; set; }
    public decimal? ElectricityReadingStart { get; set; }
    public decimal? ElectricityReadingEnd { get; set; }
    public decimal? ElectricityPriceUnit { get; set; }
    public decimal ElectricityPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public List<BookingLogEntry> LogEntries { get; set; }
    public List<BookingDocumentEntry> Documents { get; set; }

    public List<Notification> Notifications { get; set; }

    public AlertType? Alert { get; set; }
    public string AlertMessage { get; set; }


    public void AddNotification(NotificationLevelType level, string message)
    {
      Notifications.Add(new Notification { Level = level, Message = message });
    }

    public async Task UpdateCalculatedValues(
      IBookingProvider bookingProvider,
      ITenantCategoryProvider tenantCategoryProvider, 
      BookingSettings settings)
    {
      Alert = null;

      if (BookingState == BookingDataModel.BookingStateType.Requested)
      {
        Alert = AlertType.New;
        AlertMessage = "Ny reservation.";
      }
      else if (BookingState == BookingDataModel.BookingStateType.Approved)
      {
        if (!WelcomeLetterIsSent && DateTime.Now > ArrivalDate - TimeSpan.FromDays(settings.DaysBeforeArrivalForWelcomeLetter))
        {
          Alert = AlertType.Key;
          AlertMessage = "Velkomstbrev skal udsendes.";
        }
        else if (DateTime.Now > DepartureDate)
        {
          if (!IsCheckedOut)
          {
            Alert = AlertType.AwaitingCheckout;
            AlertMessage = "Afventer slutafregning fra lejer.";
          }
          else
          {
            Alert = AlertType.Finalize;
            AlertMessage = "Lejemålet er overstået og skal afsluttes.";
          }
        }
      }

      Notifications = new List<Notification>();

      var tenantCategory = await tenantCategoryProvider.GetTenantCategoryById(TenantCategoryId);
      DateTime lastAllowedArrivalDate = CreatedDate.AddMonths(tenantCategory.AllowedBookingFutureMonths);

      if (ArrivalDate >= lastAllowedArrivalDate)
        AddNotification(NotificationLevelType.Warning, $"Bemærk at ankomstdatoen ligger mere end de tilladte {tenantCategory.AllowedBookingFutureMonths} måneder efter at ansøgningen blev oprettet. " +
          $"Datoen er beregnet på baggrund af lejerkategorien.");

      if (RentalPrice == null)
        AddNotification(NotificationLevelType.Warning, "Bemærk at der endnu ikke er aftalt nogen pris.");

      // Look up overlap for active bookings (no need to add warning for historic bookings)
      if (BookingState != BookingDataModel.BookingStateType.Closed)
      {
        var overlappingBookings = await bookingProvider.FindBookingsInInterval(new Queries.SearchBookingSummariesQuery
        {
          Start = ArrivalDate,
          End = DepartureDate
        });

        foreach (var booking in overlappingBookings)
        {
          // Do not include self
          if (booking.Id != Id && booking.CollidesWith(this))
          {
            AddNotification(NotificationLevelType.Warning, $"Denne reservation overlapper med reservation nr. {booking.BookingNumber} den {booking.ArrivalDate.ToShortDateString()} til {booking.DepartureDate.ToShortDateString()}.");
          }
        }
      }

      if (BookingState == BookingDataModel.BookingStateType.Closed)
        AddNotification(NotificationLevelType.Information, $"Denne reservation er afsluttet ({GetSingleWordStateDescription()}).");
    }


    public string GetSingleWordStateDescription()
    {
      if (IsRejected)
        return "afvist";
      else if (IsApproved)
        return "godkendt";
      else if (BookingState == BookingDataModel.BookingStateType.Requested)
        return "forespørgsel";
      else if (BookingState == BookingDataModel.BookingStateType.Approved)
        return "godkendt";
      else if (BookingState == BookingDataModel.BookingStateType.Closed)
        return "afsluttet";
      else
        return "under behandling";
    }


    public IEnumerable<CalendarBookingDay> ExpandDays()
    {
      for (DateTime date = ArrivalDate; date < DepartureDate.AddDays(1); date = date.AddDays(1))
      {
        if (!OnlySelectedWeekdays)
        {
          yield return new CalendarBookingDay { Date = date, Booking = this };
        }
        else if (SelectedWeekdays != null)
        {
          // Use %7 to get sunday=0
          WeekdayType wd = (WeekdayType)((int)date.DayOfWeek % 7);
          if (SelectedWeekdays.Contains(wd))
            yield return new CalendarBookingDay { Date = date, Booking = this };
        }
      }
    }


    public bool CollidesWith(BookingSummary other)
    {
      var myDates = ExpandDays().Select(b => b.Date.Date).ToHashSet();
      foreach (var otherDate in other.ExpandDays())
        if (myDates.Contains(otherDate.Date.Date))
          return true;

      return false;
    }
  }
}
