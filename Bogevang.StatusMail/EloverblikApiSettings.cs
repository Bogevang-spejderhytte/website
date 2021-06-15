using Cofoundry.Core.Configuration;

namespace Bogevang.StatusMail.Domain
{
  public class EloverblikApiSettings : IConfigurationSettings
  {
    public string BaseUrl { get; set; }
  }
}
