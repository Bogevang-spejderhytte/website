using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
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
      var summary = new BookingSummary
      {
        Id = entity.CustomEntityId,
        CreatedDate = entity.CreateDate,
        ArrivalDate = model.ArrivalDate.Value,
        DepartureDate = model.DepartureDate.Value,
        Purpose = model.Purpose,
        TenantName = model.TenantName,
        ContactName = model.ContactName,
        ContactEMail = model.ContactEMail,
        BookingState = model.BookingState.Value,
        BookingStateText = model.BookingState.GetDescription()
      };

      return new BookingCacheEntry
      {
        Entity = entity,
        DataModel = model,
        Summary = summary
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


    public async Task<IEnumerable<BookingSummary>> FindBookingsInInterval(SearchBookingSummariesQuery query)
    {
      await EnsureBookingsLoaded();

      DateTime startValue = query?.Start ?? new DateTime(2000, 1, 1);
      DateTime endValue = query?.End ?? new DateTime(3000, 1, 1);

      var filtered = BookingCache
        .Where(b => b.DataModel.ArrivalDate >= startValue && b.DataModel.ArrivalDate < endValue);

      if (query?.BookingState != null)
        filtered = filtered
          .Where(b => b.DataModel.BookingState == query.BookingState);

      if (query?.OrderBy == SearchBookingSummariesQuery.OrderByType.ArrivalDate)
      {
        if (query?.SortDirection == SearchBookingSummariesQuery.SortDirectionType.Asc)
          filtered = filtered.OrderBy(b => b.DataModel.ArrivalDate);
        else
          filtered = filtered.OrderByDescending(b => b.DataModel.ArrivalDate);
      }
      else
      {
        if (query?.SortDirection == SearchBookingSummariesQuery.SortDirectionType.Asc)
          filtered = filtered.OrderBy(b => b.Entity.CreateDate);
        else
          filtered = filtered.OrderByDescending(b => b.Entity.CreateDate);
      }

      return filtered.Select(b => b.Summary);
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
