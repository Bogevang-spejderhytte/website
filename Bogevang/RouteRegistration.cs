using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bogevang
{
  public class RouteRegistration : IRouteRegistration
  {
    void IRouteRegistration.RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
      routeBuilder.MapControllerRoute(
          name: "bookingDocuments",
          pattern: "booking-documents/{documentId}",
          defaults: new { controller = "BookingDocument", action = "Document" });

      routeBuilder.MapControllerRoute(
          name: "adminBookings",
          pattern: "admin-bookings/{action}",
          defaults: new { controller = "AdminBookings" });
    }
  }
}
