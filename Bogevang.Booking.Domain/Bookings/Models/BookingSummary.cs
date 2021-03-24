using Bogevang.Booking.Domain.Bookings.CustomEntities;
using System;

namespace Bogevang.Booking.Domain.Bookings.Models
{
  public class BookingSummary
  {
    public enum AlertType
    {
      New,
      Key,
      Finalize
    }

    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public string Purpose { get; set; }
    public string TenantName { get; set; }
    public string ContactName { get; set; }
    public string ContactEMail { get; set; }
    public BookingDataModel.BookingStateType BookingState { get; set; }
    public string BookingStateText { get; set; }
    public string EditUrl { get; set; }

    public AlertType? Alert =>
      BookingState == BookingDataModel.BookingStateType.Requested
        ? AlertType.New
        : BookingState == BookingDataModel.BookingStateType.Confirmed && DateTime.Now > DepartureDate
          ? AlertType.Finalize
          : BookingState == BookingDataModel.BookingStateType.Confirmed && DateTime.Now > ArrivalDate - TimeSpan.FromDays(10)
            ? AlertType.Key
            : (AlertType?)null;
  }
}
