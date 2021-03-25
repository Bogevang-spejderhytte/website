using Antlr4.StringTemplate;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Domain;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Bogevang.Templates.Domain
{
  public class TemplateProvider : ITemplateProvider
  {
    private readonly IContentRepository ContentRepository;


    public TemplateProvider(
      IContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }


    public async Task<TemplateDataModel> GetTemplateByName(string name)
    {
      var entity = await ContentRepository
        .CustomEntities()
        .GetByUrlSlug(TemplateCustomEntityDefinition.DefinitionCode, name)
        .AsRenderSummary()
        .ExecuteAsync();
      var template = (TemplateDataModel)entity.Model;

      return template;
    }

    
    public string MergeText(string text, object mergeData)
    {
      Template template = new Template(text, '$', '$');
      template.Group.RegisterRenderer(typeof(DateTime), new DateRenderer());

      foreach (var property in mergeData.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
      {
        string name = property.Name;
        object value = property.GetValue(mergeData);
        template.Add(name, value);
      }

      return template.Render();
    }
  }
}
