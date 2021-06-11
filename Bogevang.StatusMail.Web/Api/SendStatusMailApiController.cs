using System.Threading.Tasks;
using Bogevang.StatusMail.Domain.Commands;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;

namespace Bogevang.StatusMail.Web.Api
{
  [Route("api/send-status-mail")]
  public class SendStatusMailApiController : ControllerBase
  {
    private readonly IApiResponseHelper ApiResponseHelper;


    public SendStatusMailApiController(
      IApiResponseHelper apiResponseHelper)
    {
      ApiResponseHelper = apiResponseHelper;
    }


    [HttpPost]
    public async Task<JsonResult> Post()
    {
      var cmd = new SendStatusMailCommand();
      return await ApiResponseHelper.RunCommandAsync(cmd);
    }
  }
}
