using System;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Bogevang.Common.AdminSettings
{
  public class AdminSettingsProvider : IAdminSettingsProvider
  {
    private readonly IContentRepository ContentRepository;


    public AdminSettingsProvider(
      IContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }


    public async Task<string> GetSetting(string name)
    {
      var customEntityQuery = new GetCustomEntityRenderSummariesByUrlSlugQuery(AdminSettingCustomEntityDefinition.DefinitionCode, name);
      var settings = await ContentRepository.ExecuteQueryAsync(customEntityQuery);

      var setting = settings?.FirstOrDefault();

      if (setting == null)
        throw new ArgumentException($"Could not find any admin setting with name '{name}'.");

      return ((AdminSettingDataModel)setting.Model).Value;
    }
  }
}
