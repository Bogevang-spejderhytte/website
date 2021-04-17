using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using Bogevang.Booking.Domain.TenantCategories;
using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Bogevang.Common.Utility;
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
    private readonly IPermissionValidationService PermissionValidationService;
    private readonly ITenantCategoryProvider TenantCategoryProvider;
    private readonly IContentRouteLibrary ContentRouteLibrary;
    private readonly BookingSettings BookingSettings;


    public BookingProvider(
      IAdvancedContentRepository contentRepository,
      IPermissionValidationService permissionValidationService,
      ITenantCategoryProvider tenantCategoryProvider,
      IContentRouteLibrary contentRouteLibrary,
      BookingSettings bookingSettings)
      : base(contentRepository)
    {
      PermissionValidationService = permissionValidationService;
      TenantCategoryProvider = tenantCategoryProvider;
      ContentRouteLibrary = contentRouteLibrary;
      BookingSettings = bookingSettings;
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

      string checkoutUrl = ContentRouteLibrary.ToAbsolute(BookingSettings.CheckoutUrlPath + "?id=" + entity.CustomEntityId + "&token=" + model.TenantSelfServiceToken);

      var summary = new BookingSummary
      {
        Id = entity.CustomEntityId,
        BookingNumber = model.BookingNumber,
        CreatedDate = entity.CreateDate,
        ArrivalDate = model.ArrivalDate.Value,
        OnlySelectedWeekdays = model.OnlySelectedWeekdays,
        SelectedWeekdays = model.SelectedWeekdays ?? new List<WeekdayType>(),
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
        Deposit = model.Deposit,
        DepositReceived = model.DepositReceived,
        BookingState = model.BookingState.Value,
        BookingStateText = model.BookingState.GetDescription(),
        IsApproved = model.IsApproved,
        IsRejected = model.IsRejected,
        WelcomeLetterIsSent = model.WelcomeLetterIsSent,
        IsCheckedOut = model.IsCheckedOut,
        IsArchived = model.IsArchived,
        TenantSelfServiceToken = model.TenantSelfServiceToken,
        CheckoutUrl = checkoutUrl,
        ElectricityReadingStart = model.ElectricityReadingStart,
        ElectricityReadingEnd = model.ElectricityReadingEnd,
        ElectricityPriceUnit = model.ElectricityPriceUnit,
        ElectricityPrice = model.ElectricityPrice,
        TotalPrice = model.TotalPrice,
        Documents = model.Documents,
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
        await entry.Summary.UpdateCalculatedValues(this, TenantCategoryProvider, BookingSettings);
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
      return result?.DataModel;
    }


    public async Task<BookingSummary> GetBookingSummaryById(int bookingId)
    {
      await EnsureCacheLoaded();

      var result = Cache.FirstOrDefault(b => b.Entity.CustomEntityId == bookingId);
      return result?.Summary;
    }


    public async Task<IList<KeyValuePair<int, BookingDataModel>>> FindBookingDataInInterval(SearchBookingSummariesQuery query)
    {
      return (await FindBookingsInInterval_Internal(query))
        .Select(b => new KeyValuePair<int,BookingDataModel>(b.Entity.CustomEntityId, b.DataModel)).ToList();
    }


    public async Task<IList<BookingSummary>> FindBookingsInInterval(SearchBookingSummariesQuery query)
    {
      return (await FindBookingsInInterval_Internal(query)).Select(b => b.Summary).ToList();
    }


    protected async Task<IEnumerable<CacheEntry>> FindBookingsInInterval_Internal(SearchBookingSummariesQuery query)
    {
      await EnsureCacheLoaded();

      if (!string.IsNullOrEmpty(query.BookingNumber))
      {
        if (int.TryParse(query.BookingNumber, out int bookingNumber))
        {
          var booking = Cache.FirstOrDefault(b => b.DataModel.BookingNumber == bookingNumber);
          if (booking == null)
            return null;
          return new CacheEntry[] { booking };
        }
        else
          return null;
      }

      DateTime startValue = query?.Start ?? new DateTime(2000, 1, 1);
      DateTime endValue = query?.End ?? new DateTime(3000, 1, 1);

      if (query.Year != null && query.Start == null && query.End == null)
      {
        if (int.TryParse(query.Year, out int year))
        {
          startValue = new DateTime(year, 1, 1);
          endValue = new DateTime(year + 1, 1, 1);
        }
      }

      var filtered = Cache
        // Calculate interval overlap
        .Where(b => b.DataModel.ArrivalDate <= endValue && b.DataModel.DepartureDate >= startValue);

      if (query?.BookingState != null)
        filtered = filtered
          .Where(b => query.BookingState.Contains(b.DataModel.BookingState.Value));

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

      return filtered;
    }
  }
}
