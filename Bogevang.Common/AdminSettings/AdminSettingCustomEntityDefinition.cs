using System;
using System.Collections.Generic;
using System.Text;
using Cofoundry.Domain;

namespace Bogevang.Common.AdminSettings
{
  public class AdminSettingCustomEntityDefinition :
    ICustomEntityDefinition<AdminSettingDataModel>,
    ICustomizedTermCustomEntityDefinition
  {
    public const string DefinitionCode = "ADMSET";

    public string CustomEntityDefinitionCode => DefinitionCode;

    public string Name => "Indstilling";

    public string NamePlural => "Indstillinger";

    public string Description => "En navngiven indstillingsværdi.";

    public bool ForceUrlSlugUniqueness => true;

    public bool HasLocale => false;

    public bool AutoGenerateUrlSlug => true;

    public bool AutoPublish => true;

    /// <summary>
    /// Here we customize the title of the setting to be displayed
    /// as 'Navn', which better describes its purpose.
    /// </summary>
    public Dictionary<string, string> CustomTerms => new Dictionary<string, string>()
        {
            { CustomizableCustomEntityTermKeys.Title, "Navn" }
        };
  }
}
