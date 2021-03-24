using Bogevang.Common.Utility;
using Cofoundry.Core.DependencyInjection;

namespace Bogevang.Common
{
  public class CommonDependencyRegistration : IDependencyRegistration
  {
    public void Register(IContainerRegister container)
    {
      container.Register<ICurrentUserProvider, CurrentUserProvider>();
    }
  }
}
