using Antlr4.StringTemplate;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections;
using System.IO;
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

      if (entity == null)
        throw new EntityNotFoundException($"Ukendt skabelonnavn '{name}'. Sørg for at oprette en skabelon i administrationsdelen med dette navn.");

      var template = (TemplateDataModel)entity.Model;
      return template;
    }


    public string GetEmbeddedTemplateByName(Assembly assembly, string name)
    {
      string resourceName = $"{assembly.GetName().Name}.Templates.{name}";
      using (Stream s = assembly.GetManifestResourceStream(resourceName))
      {
        using (var reader = new StreamReader(s))
        {
          return reader.ReadToEnd();
        }
      }
    }



    public string MergeText(string text, params object[] mergeDataSet)
    {
      Template template = new Template(text, '$', '$');
      template.Group.RegisterRenderer(typeof(DateTime), new DateRenderer());
      template.Group.RegisterRenderer(typeof(decimal), new DecimalRenderer());
      template.Group.RegisterRenderer(typeof(string), new StringRenderer());

      // Merge all data into the template, no checks for duplicates.
      foreach (var mergeData in mergeDataSet)
      {
        if (mergeData is IDictionary mergeDict)
        {
          foreach (var item in mergeDict.Keys)
            template.Add(item.ToString(), mergeDict[item]);
        }
        else
        {
          foreach (var property in mergeData.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
          {
            string name = property.Name;
            object value = property.GetValue(mergeData);
            template.Add(name, value);
          }
        }
      }

      return template.Render();
    }
  }
}
