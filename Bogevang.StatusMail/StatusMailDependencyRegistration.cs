using Cofoundry.Core.DependencyInjection;

namespace Bogevang.StatusMail.Domain
{
  public class StatusMailDependencyRegistration : IDependencyRegistration
  {
    public void Register(IContainerRegister container)
    {
      container.Register<IStatusMailProvider, StatusMailProvider>();
      container.Register<IEloverblikAPI, EloverblikAPI>();
    }
  }
}
