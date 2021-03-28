using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Bogevang.Booking.Domain.TenantCategories;
using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public class BookingProvider : 
    CachedCustomEntityProvider<BookingDataModel, BookingSummary>, 
    IBookingProvider
  {
    private readonly IAdvancedContentRepository ContentRepository;
    private readonly ITenantCategoryProvider TenantCategoryProvider;


    public BookingProvider(
      IAdvancedContentRepository contentRepository,
      ITenantCategoryProvider tenantCategoryProvider)
      : base(contentRepository)
    {
      ContentRepository = contentRepository;
      TenantCategoryProvider = tenantCategoryProvider;
    }


    protected override string CustomEntityDefinitionCode => BookingCustomEntityDefinition.DefinitionCode;


    protected override bool IsRelevantEntityCode(string entityCode)
    {
      return entityCode == BookingCustomEntityDefinition.DefinitionCode 
        || entityCode == TenantCategoryCustomEntityDefinition.DefinitionCode;
    }


    protected override CacheEntry MapEntity(CustomEntityRenderSummary entity)
    {
      var model = (BookingDataModel)entity.Model;
      var summary = new BookingSummary
      {
        Id = entity.CustomEntityId,
        CreatedDate = entity.CreateDate,
        ArrivalDate = model.ArrivalDate.Value,
        DepartureDate = model.DepartureDate.Value,
        Purpose = model.Purpose,
        TenantCategoryId = model.TenantCategoryId.Value,
        TenantName = model.TenantName,
        ContactName = model.ContactName,
        ContactPhone = model.ContactPhone,
        ContactAddress = model.ContactAddress,
        ContactCity = model.ContactCity,
        ContactEMail = model.ContactEMail,
        Comments = model.Comments,
        RentalPrice = model.RentalPrice,
        BookingState = model.BookingState.Value,
        BookingStateText = model.BookingState.GetDescription(),
        IsApproved = model.IsApproved,
        IsRejected = model.IsRejected,
        WelcomeLetterIsSent = model.WelcomeLetterIsSent,
        LogEntries = model.LogEntries
      };

      return new CacheEntry
      {
        Entity = entity,
        DataModel = model,
        Summary = summary
      };
    }


    protected override async Task PostProcessCache()
    {
      foreach (var entry in Cache)
        await entry.Summary.UpdateCalculatedValues(this, TenantCategoryProvider);
      Cache.Sort(CompareBookingByArrivalDate);
    }

    
    private int CompareBookingByArrivalDate(CacheEntry x, CacheEntry y)
    {
      if (x.DataModel.ArrivalDate < y.DataModel.ArrivalDate)
        return -1;
      else if (x.DataModel.ArrivalDate > y.DataModel.ArrivalDate)
        return 1;
      else
        return 0;
    }

    
    public async Task<BookingDataModel> GetBookingById(int bookingId)
    {
      await EnsureCacheLoaded();

      var result = Cache.FirstOrDefault(b => b.Entity.CustomEntityId == bookingId);
      if (result == null)
        throw new EntityNotFoundException($"No booking with ID {bookingId}.");
      return result.DataModel;
    }


    public async Task<BookingSummary> GetBookingSummaryById(int bookingId)
    {
      await EnsureCacheLoaded();

      var result = Cache.FirstOrDefault(b => b.Entity.CustomEntityId == bookingId);
      if (result == null)
        throw new EntityNotFoundException($"No booking with ID {bookingId}.");
      return result.Summary;
    }


    public async Task<IList<BookingSummary>> FindBookingsInInterval(SearchBookingSummariesQuery query)
    {
      await EnsureCacheLoaded();

      DateTime startValue = query?.Start ?? new DateTime(2000, 1, 1);
      DateTime endValue = query?.End ?? new DateTime(3000, 1, 1);

      var filtered = Cache
        // Calculate interval overlap
        .Where(b => b.DataModel.ArrivalDate <= endValue && b.DataModel.DepartureDate >= startValue);

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

      return filtered.Select(b => b.Summary).ToList();
    }


  }
}
