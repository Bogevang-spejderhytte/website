using System;

namespace Bogevang.Booking.Domain.Bookings.CustomEntities
{
  public class BookingDocumentEntry
  {
    public DateTime CreatedDate { get; set; }
    public string Title { get; set; }
    public int DocumentId { get; set; }
  }
}
