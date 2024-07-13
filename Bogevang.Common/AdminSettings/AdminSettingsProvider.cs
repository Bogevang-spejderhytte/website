using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
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


    async Task<string> IAdminSettingsProvider.GetSetting(string name)
    {
      return await GetSetting(name);
    }
    
    
    async Task<decimal> IAdminSettingsProvider.GetDecimalSetting(string name)
    {
      var s = await GetSetting(name);
      if (string.IsNullOrEmpty(s))
        throw new EntityNotFoundException($"No value set for admin setting '{name}'.");
      if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
        return d;
      throw new EntityNotFoundException($"Invalid value '{s}' for admin setting '{name}'.");
    }


    private async Task<string> GetSetting(string name)
    {
      var customEntityQuery = new GetCustomEntityRenderSummariesByUrlSlugQuery(AdminSettingCustomEntityDefinition.DefinitionCode, name);
      var settings = await ContentRepository.ExecuteQueryAsync(customEntityQuery);

      var setting = settings?.FirstOrDefault();

      if (setting == null)
        throw new EntityNotFoundException($"Could not find any admin setting with name '{name}'.");

      return ((AdminSettingDataModel)setting.Model).Value;
    }
  }
}
