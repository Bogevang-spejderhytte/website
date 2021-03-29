using Bogevang.Booking.Domain.Bookings;
using Bogevang.Booking.Website.ViewModels;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingCheckoutViewComponent : ViewComponent
  {
    private readonly IBookingProvider BookingProvider;


    public BookingCheckoutViewComponent(IBookingProvider bookingProvider)
    {
      BookingProvider = bookingProvider;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
      if (Request.Query.TryGetValue("id", out var id_s)
        && int.TryParse(id_s, out int id)
        && Request.Query.TryGetValue("token", out var token))
      {
        var booking = await BookingProvider.GetBookingSummaryById(id);

        if (token != booking.TenantSelfServiceToken)
          throw new AuthenticationFailedException("Ugyldigt eller manglende adgangsnøgle");
        
        return View(new BookingCheckoutViewModel
        {
          BookingId = booking.Id,
          ArrivalDate = booking.ArrivalDate,
          DepartureDate = booking.DepartureDate,
          ContactName = booking.ContactName
        });
      }

      return View(new BookingCheckoutViewModel());
    }
  }
}
