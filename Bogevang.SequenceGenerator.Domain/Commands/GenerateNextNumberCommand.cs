using Cofoundry.Domain.CQS;

namespace Bogevang.SequenceGenerator.Domain.Commands
{
  public class GenerateNextNumberCommand : ICommand
  {
    public string CounterName { get; set; }
    
    [OutputValue]
    public int OutputValue { get; set; }
  }
}
