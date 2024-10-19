using Cofoundry.Domain;

namespace Bogevang.Common.Utility
{
  public class CurrentUserDetails
  {
    public bool IsLoggedIn { get; set; }
    public UserSummary User { get; set; }
    public RoleDetails Role { get; set; }
  }
}
