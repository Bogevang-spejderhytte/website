using Bogevang.Booking.Domain.Models;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Queries
{
  public class SearchBookingSummariesQuery
      : SimplePageableQuery
      , IQuery<PagedQueryResult<BookingSummary>>
  {
  }
}
