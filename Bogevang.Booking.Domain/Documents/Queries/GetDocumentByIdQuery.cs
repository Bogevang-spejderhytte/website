using Bogevang.Booking.Domain.Documents.Entities;
using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Documents.Queries
{
  public class GetDocumentByIdQuery : IQuery<DocumentSummary>
  {
    public int DocumentId { get; set; }
  }
}
