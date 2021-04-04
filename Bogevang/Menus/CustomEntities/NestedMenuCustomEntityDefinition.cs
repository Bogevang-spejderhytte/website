using Cofoundry.Domain;
using System.Collections.Generic;

namespace Bogevang.Menus.CustomEntities
{
  public class NestedMenuCustomEntityDefinition :
    ICustomEntityDefinition<NestedMenuDataModel>,
    ICustomizedTermCustomEntityDefinition
  {
    public const string DefinitionCode = "MNUNST";

    public string CustomEntityDefinitionCode => DefinitionCode;

    public string Name => "Menuer";

    public string NamePlural => "Menuer";

    public string Description => "En to-niveau menu";

    public bool ForceUrlSlugUniqueness => true;

    public bool HasLocale => false;

    public bool AutoGenerateUrlSlug => true;

    public bool AutoPublish => false;

    /// <summary>
    /// Here we customize the title of the menu to be displayed
    /// as 'Identifier', which better describes its purpose.
    /// </summary>
    public Dictionary<string, string> CustomTerms => new Dictionary<string, string>()
        {
            { CustomizableCustomEntityTermKeys.Title, "Navn" }
        };
  }
}
