using Cofoundry.Core;
using Cofoundry.Core.AutoUpdate;
using System.Collections.Generic;

namespace Bogevang.SequenceGenerator.Domain.Install
{
  public class UpdatePackageFactory : BaseDbOnlyUpdatePackageFactory
  {
    /// <summary>
    /// The module identifier should be unique to this installation
    /// and usually indicates the application or plugin being updated
    /// </summary>
    public override string ModuleIdentifier => "SequenceGenerator";


    /// <summary>
    /// Here we can add any modules that this installation is dependent
    /// on. In this case we are dependent on the Cofoundry installation
    /// being run before this one
    /// </summary>
    public override ICollection<string> DependentModules { get; } = new string[] { CofoundryModuleInfo.ModuleIdentifier };
  }
}
