using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.StatusMail.Domain
{
  public interface IStatusMailProvider
  {
    Task<string> BuildStatusMessage();
    Task<IDictionary<string, object>> BuildStatusContent();
  }
}
