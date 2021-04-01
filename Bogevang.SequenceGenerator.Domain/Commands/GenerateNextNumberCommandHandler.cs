using Bogevang.SequenceGenerator.Domain.Data;
using Bogevang.SequenceGenerator.Domain.Entities;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Bogevang.SequenceGenerator.Domain.Commands
{
  public class GenerateNextNumberCommandHandler 
    : ICommandHandler<GenerateNextNumberCommand>,
      IIgnorePermissionCheckHandler
  {
    private readonly SequenceDbContext DbContext;
    
    
    public GenerateNextNumberCommandHandler(SequenceDbContext dbContext)
    {
      DbContext = dbContext;
    }


    public async Task ExecuteAsync(GenerateNextNumberCommand command, IExecutionContext executionContext)
    {
      SequenceCounter counter = await DbContext.Counters.FindAsync(command.CounterName);

      if (counter == null)
      {
        counter = new SequenceCounter { Name = command.CounterName, Counter = 1 };
        await DbContext.Counters.AddAsync(counter);
      }
      else
      {
        counter.Counter++;
      }

      await DbContext.SaveChangesAsync();

      command.OutputValue = counter.Counter;
    }
  }
}
