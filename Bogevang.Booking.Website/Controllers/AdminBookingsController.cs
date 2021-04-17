using Bogevang.Booking.Domain.Bookings.Commands;
using Bogevang.Booking.Website.ViewModels;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.Controllers
{
  public class AdminBookingsController : Controller
  {
    public ICommandExecutor CommandExecutor;


    public AdminBookingsController(
      ICommandExecutor executor)
    {
      CommandExecutor = executor;
    }


    [HttpGet]
    public ActionResult Start()
    {
      return View();
    }


    [HttpPost]
    public async Task<ActionResult> Anonymize()
    {
      var cmd = new AnonymizeBookingsCommand();
      await CommandExecutor.ExecuteAsync(cmd);
      return View("Done", new AdminResultViewModel { Message = $"{cmd.AnonymizedCount} reservationer blev anonymiseret." });
    }


    //[HttpPost]
    //public async Task<ActionResult> Import(IFormFile document)
    //{
    //  await CommandExecutor.ExecuteAsync(new ImportBookingsCommand { ReadyToReadInput = document.OpenReadStream() });
    //  return View("Done", new AdminResultViewModel { Message = "alle reservationer er importeret." });
    //}


    //[HttpPost]
    //public async Task<ActionResult> DeleteAll(string confirm)
    //{
    //  if (confirm != "Slet alt")
    //    throw new ValidationException("Der står ikke 'Slet alt'");

    //  await CommandExecutor.ExecuteAsync(new DeleteAllBookingsCommand());
    //  return View("Done", new AdminResultViewModel { Message = "alle reservationer er slettet." });
    //}
  }
}
