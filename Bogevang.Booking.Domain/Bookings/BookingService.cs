using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public class BookingService : IBookingService,
    IMessageHandler<CustomEntityAddedMessage>,
    IMessageHandler<ICustomEntityContentUpdatedMessage>,
    IMessageHandler<CustomEntityDeletedMessage>
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


    private async Task ResetBookings()
    {
      await BookingsSemaphore.WaitAsync();

      try
      {
        BookingCache = null;
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

    
    public async Task HandleAsync(CustomEntityAddedMessage message)
    {
      if (message.CustomEntityDefinitionCode == BookingCustomEntityDefinition.DefinitionCode)
        await ResetBookings();
    }


    public async Task HandleAsync(ICustomEntityContentUpdatedMessage message)
    {
      if (message.CustomEntityDefinitionCode == BookingCustomEntityDefinition.DefinitionCode)
        await ResetBookings();
    }


    public async Task HandleAsync(CustomEntityDeletedMessage message)
    {
      if (message.CustomEntityDefinitionCode == BookingCustomEntityDefinition.DefinitionCode)
        await ResetBookings();
    }
  }
}
