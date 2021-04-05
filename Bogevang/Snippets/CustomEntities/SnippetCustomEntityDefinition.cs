using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Snippets.CustomEntities
{
  public class SnippetCustomEntityDefinition :
    ICustomEntityDefinition<SnippetDataModel>,
    ICustomizedTermCustomEntityDefinition
  {
    public const string DefinitionCode = "SNIPET";

    public string CustomEntityDefinitionCode => DefinitionCode;

    public string Name => "Tekst";

    public string NamePlural => "Tekster";

    public string Description => "En stump tekst som kan indsættes på hjemmesiden";

    public bool ForceUrlSlugUniqueness => true;

    public bool HasLocale => false;

    public bool AutoGenerateUrlSlug => true;

    public bool AutoPublish => false;

    /// <summary>
    /// Here we customize the title of the menu to be displayed
    /// as 'Navn', which better describes its purpose.
    /// </summary>
    public Dictionary<string, string> CustomTerms => new Dictionary<string, string>()
        {
            { CustomizableCustomEntityTermKeys.Title, "Navn" }
        };
  }
}
