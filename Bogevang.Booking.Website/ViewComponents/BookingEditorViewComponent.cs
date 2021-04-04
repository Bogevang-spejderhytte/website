using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingEditorViewComponent : ViewComponent
  {
    private readonly IPermissionValidationService PermissionValidationService;


    public BookingEditorViewComponent(
      IPermissionValidationService permissionValidationService)
    {
      PermissionValidationService = permissionValidationService;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
      if (await PermissionValidationService.HasCustomEntityPermissionAsync<CustomEntityReadPermission>(BookingCustomEntityDefinition.DefinitionCode))
        return View(new BookingDataModel());
      else
        return View("Blocked");
    }
  }
}
