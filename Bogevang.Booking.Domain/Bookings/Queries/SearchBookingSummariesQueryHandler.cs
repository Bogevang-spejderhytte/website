#if false
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Queries
{
  public class SearchBookingSummariesQueryHandler
    : IQueryHandler<SearchBookingSummariesQuery, PagedQueryResult<BookingSummary>>
    , IIgnorePermissionCheckHandler
  {
    private readonly IContentRepository ContentRepository;
    private readonly IHttpContextAccessor HttpContext;

    public SearchBookingSummariesQueryHandler(
      IContentRepository contentRepository,
      IHttpContextAccessor httpContext)
    {
      ContentRepository = contentRepository;
      HttpContext = httpContext;
    }


    public async Task<PagedQueryResult<BookingSummary>> ExecuteAsync(
      SearchBookingSummariesQuery query, 
      IExecutionContext executionContext)
    {
      var customEntityQuery = new SearchCustomEntityRenderSummariesQuery()
      {
        CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
        PageNumber = 1,
        PageSize = 50,
        PublishStatus = PublishStatusQuery.Published
      };

      var entities = await ContentRepository
        .CustomEntities()
        .Search()
        .AsRenderSummaries(customEntityQuery)
        .ExecuteAsync();

      return MapBookings(entities);
    }


    private PagedQueryResult<BookingSummary> MapBookings(PagedQueryResult<CustomEntityRenderSummary> entities)
    {
      var bookings = new List<BookingSummary>(entities.Items.Count);

      var request = HttpContext.HttpContext.Request;
      string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
      string editUrl = baseUrl + "/reservationer/edit#";

      foreach (var entity in entities.Items)
      {
        var model = (BookingDataModel)entity.Model;
        var booking = new BookingSummary
        {
          Id = entity.CustomEntityId,
          ArrivalDate = model.ArrivalDate?.ToShortDateString(),
          DepartureDate = model.DepartureDate?.ToShortDateString(),
          Purpose = model.Purpose,
          TenantName = model.TenantName,
          ContactName = model.ContactName,
          ContactEMail = model.ContactEMail,
          BookingState = model.BookingState.GetDescription(),
          EditUrl = editUrl + entity.CustomEntityId
        };

        bookings.Add(booking);
      }

      return entities.ChangeType(bookings);
    }
  }
}
#endif