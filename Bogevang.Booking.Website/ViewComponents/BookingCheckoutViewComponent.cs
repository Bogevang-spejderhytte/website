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
        && int.TryParse(id_s, out int id))
      {
        var booking = await BookingProvider.GetBookingSummaryById(id);

        Request.Query.TryGetValue("token", out var token);
        if (token != booking.TenantSelfServiceToken)
          throw new AuthenticationFailedException("Ugyldig eller manglende adgangsnøgle");
        
        return View(new BookingCheckoutViewModel
        {
          BookingId = booking.Id,
          BookingNumber = booking.BookingNumber,
          ArrivalDate = booking.ArrivalDate,
          DepartureDate = booking.DepartureDate,
          ContactName = booking.ContactName,
          Deposit = booking.Deposit ?? 0,
          ElectricityPriceUnit = booking.ElectricityPriceUnit
        });
      }

      return View(new BookingCheckoutViewModel());
    }
  }
}
