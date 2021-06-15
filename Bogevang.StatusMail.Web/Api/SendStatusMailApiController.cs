using System;
using System.Threading.Tasks;
using Bogevang.StatusMail.Domain.Commands;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

namespace Bogevang.StatusMail.Web.Api
{
  [Route("api/send-status-mail")]
  public class SendStatusMailApiController : ControllerBase
  {
    private readonly IServiceProvider ServiceProvider;


    public SendStatusMailApiController(IServiceProvider serviceProvider)
    {
      ServiceProvider = serviceProvider;
    }


    [HttpPost]
    public object Post()
    {
      // Start as background task - don't wait for completing as it may take some time.
      _ = RunCommandInBackground(ServiceProvider);
      return Ok();
    }


    public async Task RunCommandInBackground(IServiceProvider serviceProvider)
    {
      using (var scope = ServiceProvider.CreateScope())
      {
        var cmd = new SendStatusMailCommand();
        await scope.ServiceProvider.GetRequiredService<IDomainRepository>().ExecuteCommandAsync(cmd);
      }
    }
  }
}
