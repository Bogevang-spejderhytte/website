using System;
using static Bogevang.Booking.Domain.CustomEntities.BookingDataModel;

namespace Bogevang.Booking.Domain.Models
{
  public class BookingDetails
  {
    public string ArrivalDate { get; set; }
    public string DepartureDate { get; set; }
    public int TenantCategoryId { get; set; }
    public string TenantName { get; set; }
    public string Purpose { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactAddress { get; set; }
    public string ContactCity { get; set; }
    public string ContactEMail { get; set; }
    public string Comments { get; set; }
    public decimal RentalPrice { get; set; }
    public BookingStateType BookingState { get; set; }
    public bool DepositReceived { get; set; }
    public bool PaymentReceived { get; set; }
    public bool DepositReturned { get; set; }
  }
}
