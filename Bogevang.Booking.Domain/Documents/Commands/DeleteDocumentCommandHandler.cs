using Bogevang.Booking.Domain.Documents.Data;
using Bogevang.Booking.Domain.Documents.Entities;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Documents.Commands
{
  public class DeleteDocumentCommandHandler 
    : ICommandHandler<DeleteDocumentCommand>,
      IIgnorePermissionCheckHandler // FIXME
  {
    private readonly DocumentDbContext DbContext;
    private readonly ITransactionScopeManager TransactionScopeFactory;


    public DeleteDocumentCommandHandler(
        DocumentDbContext dbContext,
        ITransactionScopeManager transactionScopeFactory
        )
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
