using System;
using System.Threading.Tasks;
using Bogevang.StatusMail.Domain;
using Bogevang.StatusMail.Domain.Commands;
using Cofoundry.Domain;
using Cofoundry.Plugins.ErrorLogging.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace Bogevang.StatusMail.Web.Api
{
  [Route("api/send-status-mail")]
  public class SendStatusMailApiController : ControllerBase
  {
    private readonly IServiceProvider ServiceProvider;
    private readonly StatusMailSettings StatusMailSettings;


    public SendStatusMailApiController(
      IServiceProvider serviceProvider,
      StatusMailSettings statusMailSettings)
    {
      ServiceProvider = serviceProvider;
      StatusMailSettings = statusMailSettings;
    }

    // TEST like this:
    // curl -d "accessToken=TODO" -X POST https://localhost:44313/api/send-status-mail
    // (access token is from appsettings.json)

    [HttpPost]
    [Consumes("application/x-www-form-urlencoded")]
    public object Post([FromForm] string accessToken)
    {
      if (accessToken != StatusMailSettings.AccessToken)
        return Unauthorized();

      // Start as background task - don't wait for completing as it may take some time.
      _ = RunCommandInBackground(ServiceProvider);
      return Ok();
    }


    public async Task RunCommandInBackground(IServiceProvider serviceProvider)
    {
      using (var scope = ServiceProvider.CreateScope())
      {
        IErrorLoggingService logginService = scope.ServiceProvider.GetRequiredService<IErrorLoggingService>();
        try
        {
          var cmd = new SendStatusMailCommand();
          await scope.ServiceProvider.GetRequiredService<IDomainRepository>().ExecuteCommandAsync(cmd);
        }
        catch (Exception ex)
        {
          await logginService.LogAsync(ex);

          // Nothing more to do here as we run this async, so the request has completed a long time ago.
          // Just pray for the log statement to tell what is wrong.
        }
      }
    }
  }
}
