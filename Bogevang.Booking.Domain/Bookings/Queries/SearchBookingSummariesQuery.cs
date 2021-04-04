using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bogevang.Booking.Domain.Bookings.Queries
{
  public class SearchBookingSummariesQuery : IQuery<IList<BookingSummary>>
  {
    public enum OrderByType
    {
      [Description("Ankomstdato")]
      ArrivalDate,

      [Description("Oprettelsesdato")]
      CreatedDate
    }


    public enum SortDirectionType
    {
      [Description("Stigende")]
      Asc,
      
      [Description("Faldende")]
      Desc
    }


    public string BookingNumber { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public string Year { get; set; }
    public OrderByType OrderBy { get; set; }
    public SortDirectionType SortDirection { get; set; }
    public ICollection<BookingDataModel.BookingStateType> BookingState { get; set; }
  }
}
