using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Documents.Commands
{
  /// <summary>
  /// Adds a new document.
  /// </summary>
  public class AddDocumentCommand : ICommand, ILoggableCommand
  {
    /// <summary>
    /// The file source to retreive the document data from. The
    /// IUploadedFile abstarction is used here to support multiple
    /// types of file source.
    /// </summary>
    [Required]
    [ValidateObject]
    public byte[] Body { get; set; }

    /// <summary>
    /// A short descriptive title of the document (100 characters).
    /// </summary>
    [StringLength(100)]
    [Required]
    public string Title { get; set; }


    /// <summary>
    /// Mime type of file.
    /// </summary>
    [StringLength(100)]
    [Required]
    public string MimeType { get; set; }


    #region Output

    /// <summary>
    /// The database id of the newly created document.
    /// This is set after the command has been run.
    /// </summary>
    [OutputValue]
    public int OutputDocumentId { get; set; }

    #endregion
  }
}
