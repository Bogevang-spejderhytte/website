using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Bogevang.Common.Utility
{
  public class CurrentUserProvider : ICurrentUserProvider
  {
    private CurrentUserDetails UserDetails = null;

    private readonly IUserContextService UserContextServiceService;
    private readonly IQueryExecutor QueryExecutor;

    public CurrentUserProvider(
        IUserContextService userContextServiceService,
        IQueryExecutor queryExecutor
        )
    {
      UserContextServiceService = userContextServiceService;
      QueryExecutor = queryExecutor;
    }


    public async Task<CurrentUserDetails> GetAsync()
    {
      if (UserDetails == null)
      {
        await InitializeAsync();
      }

      return UserDetails;
    }


    private async Task InitializeAsync()
    {
      var details = new CurrentUserDetails();
      var userContext = await UserContextServiceService.GetCurrentContextAsync();
      details.Role = await QueryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(userContext.RoleId));

      if (userContext.UserId.HasValue)
      {
        var query = new GetUserSummaryByIdQuery(userContext.UserId.Value);
        details.User = await QueryExecutor.ExecuteAsync(query);
        details.IsLoggedIn = true;
      }
      else
      {
        details.User = new UserSummary
        {
          DisplayName = "Anonym",
          Username = "Anonym",
          FirstName = "Anonym",
          LastName = "",
          Email = null,
          UserArea = null,
          UserId = 0
        };
      }

      UserDetails = details;
    }
  }
}
