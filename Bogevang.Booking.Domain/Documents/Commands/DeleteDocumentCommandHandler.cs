using Bogevang.Booking.Domain.Documents.Data;
using Cofoundry.Core.Data;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Documents.Commands
{
  public class DeleteDocumentCommandHandler :
    ICommandHandler<DeleteDocumentCommand>,
    IIgnorePermissionCheckHandler // Permission must be enforced by calling code (document handling is always executed from bookings)
  {
    private readonly DocumentDbContext DbContext;
    private readonly ITransactionScopeManager TransactionScopeFactory;


    public DeleteDocumentCommandHandler(
        DocumentDbContext dbContext,
        ITransactionScopeManager transactionScopeFactory)
    {
      DbContext = dbContext;
      TransactionScopeFactory = transactionScopeFactory;
    }


    public async Task ExecuteAsync(DeleteDocumentCommand command, IExecutionContext executionContext)
    {
      using (var scope = TransactionScopeFactory.Create(DbContext))
      {
        var document = await DbContext
            .Documents
            .FindAsync(command.Id);

        if (document != null)
        {
          DbContext.Documents.Remove(document);

          await DbContext.SaveChangesAsync();
          await scope.CompleteAsync();
        }
      }
    }
  }
}
