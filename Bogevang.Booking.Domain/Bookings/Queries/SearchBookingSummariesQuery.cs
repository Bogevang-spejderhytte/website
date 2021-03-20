using Bogevang.Booking.Domain.Bookings.Models;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Queries
{
  public class SearchBookingSummariesQuery
      : SimplePageableQuery
      , IQuery<PagedQueryResult<BookingSummary>>
  {
  }
}
