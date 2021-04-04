using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Queries
{
  public class SearchBookingSummariesQueryHandler :
    IQueryHandler<SearchBookingSummariesQuery, IList<BookingSummary>>,
    IPermissionRestrictedQueryHandler<SearchBookingSummariesQuery, IList<BookingSummary>>
  {
    private readonly ICustomEntityDefinitionRepository CustomEntityDefinitionRepository;
    private readonly IBookingProvider BookingProvider;


    public SearchBookingSummariesQueryHandler(
      ICustomEntityDefinitionRepository customEntityDefinitionRepository,
      IBookingProvider bookingProvider)
    {
      CustomEntityDefinitionRepository = customEntityDefinitionRepository;
      BookingProvider = bookingProvider;
    }


    public async Task<IList<BookingSummary>> ExecuteAsync(SearchBookingSummariesQuery query, IExecutionContext executionContext)
    {
      return await BookingProvider.FindBookingsInInterval(query);
    }


    public IEnumerable<IPermissionApplication> GetPermissions(SearchBookingSummariesQuery query)
    {
      var definition = CustomEntityDefinitionRepository.GetByCode(BookingCustomEntityDefinition.DefinitionCode);

      yield return new CustomEntityReadPermission(definition);
    }
  }
}
