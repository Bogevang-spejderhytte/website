using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingListViewComponent : ViewComponent
  {
    private readonly IPermissionValidationService PermissionValidationService;


    public BookingListViewComponent(
      IPermissionValidationService permissionValidationService)
    {
      PermissionValidationService = permissionValidationService;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
      if (await PermissionValidationService.HasCustomEntityPermissionAsync<CustomEntityReadPermission>(BookingCustomEntityDefinition.DefinitionCode))
        return View(new SearchBookingSummariesQuery());
      else
        return View("Blocked");
    }
  }
}
