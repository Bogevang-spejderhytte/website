using System.Threading.Tasks;

namespace Bogevang.Common.Utility
{
  public interface ICurrentUserProvider
  {
    Task<CurrentUserDetails> GetAsync();
  }
}
