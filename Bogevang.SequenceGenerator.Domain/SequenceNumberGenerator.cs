using Bogevang.SequenceGenerator.Domain.Commands;
using Cofoundry.Domain;
using System.Threading.Tasks;

namespace Bogevang.SequenceGenerator.Domain
{
  public class SequenceNumberGenerator : ISequenceNumberGenerator
  {
    private readonly IDomainRepository DomainRepository;


    public SequenceNumberGenerator(IDomainRepository domainRepository)
    {
      DomainRepository = domainRepository;
    }


    public async Task<int> NextNumber(string sequenceName)
    {
      var command = new GenerateNextNumberCommand { CounterName = sequenceName };
      await DomainRepository.ExecuteCommandAsync(command);
      return command.OutputValue;
    }
  }
}
