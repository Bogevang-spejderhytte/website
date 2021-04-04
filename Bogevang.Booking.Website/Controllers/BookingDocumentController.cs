using Bogevang.Booking.Domain.Documents.Data;
using Bogevang.Booking.Domain.Documents.Queries;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Controllers
{
  public class BookingDocumentController : Controller
  {
    private readonly IQueryExecutor QueryExecutor;


    public BookingDocumentController(IQueryExecutor queryExecutor)
    {
      QueryExecutor = queryExecutor;
    }


    public async Task<ActionResult> Document(int documentId)
    {
      var document = await QueryExecutor.ExecuteAsync(new GetDocumentByIdQuery { DocumentId = documentId });
      if (document == null)
        return NotFound();

      return new FileStreamResult(new MemoryStream(document.Body), document.MimeType);
    }
  }
}
