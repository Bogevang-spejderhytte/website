using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Common.Utility;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public class BookingProvider : IBookingProvider,
    IMessageHandler<CustomEntityAddedMessage>,
    IMessageHandler<ICustomEntityContentUpdatedMessage>,
    IMessageHandler<CustomEntityDeletedMessage>
  {
    private readonly IContentRepository ContentRepository;

    static SemaphoreSlim BookingsSemaphore = new SemaphoreSlim(1, 1);

    
    class BookingCacheEntry
    {
      public CustomEntityRenderSummary Entity { get; set; }
      public BookingDataModel DataModel { get; set; }
      public BookingSummary Summary { get; set; }
    }

    private static List<BookingCacheEntry> BookingCache { get; set; }


    public BookingProvider(
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

          BookingCache = allBookings
            .Select(MapBooking)
            .OrderByDescending(b => b.DataModel.ArrivalDate)
            .ToList();
        }
      }
      finally
      {
        BookingsSemaphore.Release();
      }
    }


    private BookingCacheEntry MapBooking(CustomEntityRenderSummary entity)
    {
      var model = (BookingDataModel)entity.Model;
      var booking = new BookingSummary
      {
        Id = entity.CustomEntityId,
        ArrivalDate = model.ArrivalDate.Value,
        DepartureDate = model.DepartureDate.Value,
        Purpose = model.Purpose,
        TenantName = model.TenantName,
        ContactName = model.ContactName,
        ContactEMail = model.ContactEMail,
        BookingState = model.BookingState.GetDescription()
      };

      return new BookingCacheEntry
      {
        Entity = entity,
        DataModel = model,
        Summary = booking
      };
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


    public async Task<List<BookingSummary>> FindBookingsInInterval(DateTime? start, DateTime? end)
    {
      await EnsureBookingsLoaded();

      DateTime startValue = start ?? new DateTime(2000, 1, 1);
      DateTime endValue = end ?? new DateTime(3000, 1, 1);

      var filtered =
        BookingCache
        .Where(b => b.DataModel.ArrivalDate >= startValue && b.DataModel.ArrivalDate < endValue)
        .Select(b => b.Summary)
        .ToList();

      return filtered;
    }


    #region Message handlers for keeping cache in sync

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

    #endregion
  }
}
