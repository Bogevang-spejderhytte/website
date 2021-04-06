using Bogevang.Booking.Domain.Bookings.Commands;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Controllers
{
  [Route("import-bookings")]
  public class ImportBookingsController : Controller
  {
    public ICommandExecutor CommandExecutor;


    public ImportBookingsController(
      ICommandExecutor executor)
    {
      CommandExecutor = executor;
    }


    [HttpGet]
    public ActionResult Import()
    {
      return View();
    }


    [HttpPost()]
    public async Task<ActionResult> Upload(IFormFile document)
    {
      await CommandExecutor.ExecuteAsync(new ImportBookingsCommand { ReadyToReadInput = document.OpenReadStream() });
      return View("Done");
    }
  }
}
