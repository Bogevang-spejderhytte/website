using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Documents.Data;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Documents.Queries
{
  public class GetDocumentByIdQueryHandler :
    IQueryHandler<GetDocumentByIdQuery, DocumentSummary>,
    IPermissionRestrictedQueryHandler<GetDocumentByIdQuery, DocumentSummary>
  {
    private readonly DocumentDbContext DbContext;
    private readonly ICustomEntityDefinitionRepository CustomEntityDefinitionRepository;


    public GetDocumentByIdQueryHandler(
      DocumentDbContext dbContext,
      ICustomEntityDefinitionRepository customEntityDefinitionRepository)
    {
      DbContext = dbContext;
      CustomEntityDefinitionRepository = customEntityDefinitionRepository;
    }


    public async Task<DocumentSummary> ExecuteAsync(GetDocumentByIdQuery query, IExecutionContext executionContext)
    {
      var document = await DbContext
        .Documents
        .FindAsync(query.DocumentId);

      if (document == null)
        return null;

      return new DocumentSummary
      {
        Id = document.Id,
        Title = document.Title,
        MimeType = document.MimeType,
        Body = document.Body,
        CreatedDate = document.CreatedDate
      };
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentByIdQuery query)
    {
      var definition = CustomEntityDefinitionRepository.GetByCode(BookingCustomEntityDefinition.DefinitionCode);

      yield return new CustomEntityReadPermission(definition);
    }
  }
}
