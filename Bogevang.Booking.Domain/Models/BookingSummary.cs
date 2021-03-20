using System;

namespace Bogevang.Booking.Domain.Models
{
  public class BookingSummary
  {
    public int Id { get; set; }
    public string ArrivalDate { get; set; }
    public string DepartureDate { get; set; }
    public string Purpose { get; set; }
    public string TenantName { get; set; }
    public string ContactName { get; set; }
    public string ContactEMail { get; set; }
    public string BookingState { get; set; }
    public string EditUrl { get; set; }
  }
}
