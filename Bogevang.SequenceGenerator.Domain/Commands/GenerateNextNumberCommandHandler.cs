using Bogevang.SequenceGenerator.Domain.Data;
using Bogevang.SequenceGenerator.Domain.Entities;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bogevang.SequenceGenerator.Domain.Commands
{
  public class GenerateNextNumberCommandHandler 
    : ICommandHandler<GenerateNextNumberCommand>,
      IIgnorePermissionCheckHandler
  {
    private readonly SequenceDbContext DbContext;
    private readonly ITransactionScopeManager TransactionScopeManager;


    public GenerateNextNumberCommandHandler(
      SequenceDbContext dbContext,
      ITransactionScopeManager transactionScopeManager)
    {
      DbContext = dbContext;
      TransactionScopeManager = transactionScopeManager;
    }


    public async Task ExecuteAsync(GenerateNextNumberCommand command, IExecutionContext executionContext)
    {
      using (var scope = TransactionScopeManager.Create(DbContext))
      {
        // Lock this counter to avoid concurrent requests both getting a read of the same counter value
        // - It may though fail the very first time when the counter doesn't exist yet. I'll live with that.
        string tableName = DbConstants.DefaultAppSchema + ".SequenceCounter";
        await DbContext.Database.ExecuteSqlRawAsync($"SELECT TOP 1 * FROM {tableName} WITH(HOLDLOCK,UPDLOCK) WHERE [Name] = {{0}} ", command.CounterName);

        SequenceCounter counter = await DbContext.Counters.FindAsync(command.CounterName);

        if (counter == null)
        {
          counter = new SequenceCounter { Name = command.CounterName, Counter = 1 };
          await DbContext.Counters.AddAsync(counter);
        }
        else
        {
          counter.Counter++;
          await DbContext.SaveChangesAsync();
        }

        command.OutputValue = counter.Counter;

        await scope.CompleteAsync();
      }
    }
  }
}
