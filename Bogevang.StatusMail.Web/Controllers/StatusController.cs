using Bogevang.StatusMail.Domain;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.StatusMail.Web.Controllers
{
  [Route("status/show")]
  public class StatusController : Controller
  {
    protected IStatusMailProvider StatusMailProvider { get; set; }

    protected StatusMailSettings Settings { get; set; }


    public StatusController(
      IStatusMailProvider statusMailProvider,
      StatusMailSettings settings)
    {
      StatusMailProvider = statusMailProvider;
      Settings = settings;
    }


    public async Task<object> show()
    {
      var mailContent = await StatusMailProvider.BuildStatusMessage();

      return View("Show", new HtmlString(mailContent));
    }
  }
}
