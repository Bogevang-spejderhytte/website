using Bogevang.Booking.Domain.Documents.Data;
using Bogevang.Booking.Domain.Documents.Entities;
using Cofoundry.Core.Data;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Documents.Commands
{
  public class AddDocumentCommandHandler 
    : ICommandHandler<AddDocumentCommand>,
      IIgnorePermissionCheckHandler // FIXME
  {
    private readonly DocumentDbContext DbContext;
    private readonly ITransactionScopeManager TransactionScopeFactory;


    public AddDocumentCommandHandler(
        DocumentDbContext dbContext,
        ITransactionScopeManager transactionScopeFactory
        )
    {
      DbContext = dbContext;
      TransactionScopeFactory = transactionScopeFactory;
    }


    public async Task ExecuteAsync(AddDocumentCommand command, IExecutionContext executionContext)
    {
      using (var scope = TransactionScopeFactory.Create(DbContext))
      {
        var document = new Document
        {
          Title = command.Title,
          MimeType = command.MimeType,
          Body = command.Body,
          CreatedDate = DateTime.Now,
        };

        DbContext.Documents.Add(document);
        await DbContext.SaveChangesAsync();

        command.OutputDocumentId = document.Id;

        await scope.CompleteAsync();
      }
    }
  }
}
