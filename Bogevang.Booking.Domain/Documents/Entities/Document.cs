using System;

namespace Bogevang.Booking.Domain.Documents.Entities
{
  public class Document
  {
    /// <summary>
    /// Database primary key ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Document title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Mime type for document body
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Document context
    /// </summary>
    public byte[] Body { get; set; }

    /// <summary>
    /// Created timestamp.
    /// </summary>
    public DateTime CreatedDate { get; set; }
  }
}
