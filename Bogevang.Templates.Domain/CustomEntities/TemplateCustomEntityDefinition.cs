﻿using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bogevang.Templates.Domain.CustomEntities
{
  public class TemplateCustomEntityDefinition 
    : ICustomEntityDefinition<TemplateDataModel>,
      ICustomizedTermCustomEntityDefinition
  {
    /// <summary>
    /// This constant is a convention that allows us to reference this definition code 
    /// in other parts of the application (e.g. querying)
    /// </summary>
    public const string DefinitionCode = "TMPLAT";

    /// <summary>
    /// Unique 6 letter code representing the module (the convention is to use uppercase)
    /// </summary>
    public string CustomEntityDefinitionCode => DefinitionCode;

    /// <summary>
    /// Singlar name of the entity
    /// </summary>
    public string Name => "Skabelon";

    /// <summary>
    /// Plural name of the entity
    /// </summary>
    public string NamePlural => "Skabeloner";

    /// <summary>
    /// A short description that shows up as a tooltip for the admin 
    /// panel.
    /// </summary>
    public string Description => "Skabeloner til breve.";

    /// <summary>
    /// Indicates whether the UrlSlug property should be treated
    /// as a unique property and be validated as such.
    /// </summary>
    public bool ForceUrlSlugUniqueness => true;

    /// <summary>
    /// Indicates whether the url slug should be autogenerated. If this
    /// is selected then the user will not be shown the UrlSlug property
    /// and it will be auto-generated based on the title.
    /// </summary>
    public bool AutoGenerateUrlSlug => false;

    /// <summary>
    /// Indicates whether this custom entity should always be published when 
    /// saved, provided the user has permissions to do so. Useful if this isn't
    /// the sort of entity that needs a draft state workflow
    /// </summary>
    public bool AutoPublish => true;

    /// <summary>
    /// Indicates whether the entities are partitioned by locale
    /// </summary>
    public bool HasLocale => false;

    public Dictionary<string, string> CustomTerms => new Dictionary<string, string>
    {
      [CustomizableCustomEntityTermKeys.UrlSlug] = "Skabelonnavn"
    };
  }
}
