using Cofoundry.Core.DependencyInjection;

namespace Bogevang.Templates.Domain
{
  public class TemplatesDependencyRegistration : IDependencyRegistration
  {
    public void Register(IContainerRegister container)
    {
      container.Register<ITemplateProvider, TemplateProvider>();
    }
  }
}
