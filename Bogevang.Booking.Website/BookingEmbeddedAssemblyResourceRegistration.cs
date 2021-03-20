using Cofoundry.Core.ResourceFiles;
using System.Collections.Generic;

namespace Bogevang.Booking.Website
{
  public class BookingEmbeddedAssemblyResourceRegistration : IEmbeddedResourceRouteRegistration
  {
    public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
    {
      var path = new EmbeddedResourcePath(
          GetType().Assembly,
          "/client-app",
          "/booking/scripts"
          );

      yield return path;
    }
  }
}
