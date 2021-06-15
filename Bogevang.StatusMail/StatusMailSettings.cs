using Cofoundry.Core.Configuration;

namespace Bogevang.StatusMail.Domain
{
  public class StatusMailSettings : IConfigurationSettings
  {
    public string MailReceiver { get; set; }
    public string AccessToken { get; set; }
  }
}
