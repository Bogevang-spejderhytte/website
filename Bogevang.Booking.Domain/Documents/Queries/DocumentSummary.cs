using System;

namespace Bogevang.Booking.Domain.Documents.Queries
{
  public class DocumentSummary
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string MimeType { get; set; }
    public byte[] Body { get; set; }
    public DateTime CreatedDate { get; set; }
  }
}
