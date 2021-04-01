using System.Threading.Tasks;

namespace Bogevang.SequenceGenerator.Domain
{
  public interface ISequenceNumberGenerator
  {
    Task<int> NextNumber(string sequenceName);
  }
}
