using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Domain.Bookings.Commands;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class SendBookingMailViewComponent : ViewComponent
  {
    private readonly IBookingMailService BookingMailService;
    private readonly IPermissionValidationService PermissionValidationService;


    public SendBookingMailViewComponent(
      IBookingMailService bookingMailService,
      IPermissionValidationService permissionValidationService)
    {
      BookingMailService = bookingMailService;
      PermissionValidationService = permissionValidationService;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
      if (Request.Query.TryGetValue("id", out var id_s) 
        && int.TryParse(id_s, out int id)
        && Request.Query.TryGetValue("template", out var template))
      {
        if (await PermissionValidationService.HasCustomEntityPermissionAsync<CustomEntityReadPermission>(BookingCustomEntityDefinition.DefinitionCode))
        {
          BookingMail mail = await BookingMailService.CreateBookingMail(id, template);
          return View(mail);
        }
        else
          return View("Blocked");
      }

      return View(new BookingMail());
    }
  }
}
