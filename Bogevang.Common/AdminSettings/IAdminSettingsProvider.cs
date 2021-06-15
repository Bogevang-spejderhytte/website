using System.Threading.Tasks;

namespace Bogevang.Common.AdminSettings
{
  public interface IAdminSettingsProvider
  {
    Task<string> GetSetting(string name);
  }
}
