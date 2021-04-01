using Bogevang.SequenceGenerator.Domain.Data;
using Cofoundry.Core.DependencyInjection;

namespace Bogevang.SequenceGenerator.Domain
{
  public class SequenceNumberDependencyRegistration : IDependencyRegistration
  {
    public void Register(IContainerRegister container)
    {
      container.Register<ISequenceNumberGenerator, SequenceNumberGenerator>();
      container.RegisterScoped<SequenceDbContext>();
    }
  }
}
