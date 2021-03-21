using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public class BookingService : IBookingService
  {
    private readonly IContentRepository ContentRepository;

    static SemaphoreSlim BookingsSemaphore = new SemaphoreSlim(1, 1);

    private static List<BookingDataModel> BookingCache { get; set; }


    public BookingService(
      IContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }


    private async Task EnsureBookingsLoaded()
    {
      await BookingsSemaphore.WaitAsync();

      try
      {
        if (BookingCache == null)
        {
          var allBookings = await ContentRepository
            .CustomEntities()
            .GetByDefinitionCode(BookingCustomEntityDefinition.DefinitionCode)
            .AsRenderSummary()
            .ExecuteAsync();

          BookingCache = allBookings.Select(b => (BookingDataModel)b.Model).ToList();
        }
      }
      finally
      {
        BookingsSemaphore.Release();
      }
    }


    public async Task<List<BookingDataModel>> FindBookingsInInterval(DateTime start, DateTime end)
    {
      await EnsureBookingsLoaded();

      var filtered =
        BookingCache
        .Where(b => b.ArrivalDate >= start && b.ArrivalDate < end)
        .ToList();

      return filtered;
    }
  }
}
