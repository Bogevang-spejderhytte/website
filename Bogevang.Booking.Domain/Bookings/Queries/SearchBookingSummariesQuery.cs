using Bogevang.Booking.Domain.Bookings.CustomEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Bookings.Queries
{
  public class SearchBookingSummariesQuery
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


    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public OrderByType OrderBy { get; set; }
    public SortDirectionType SortDirection { get; set; }
    public ICollection<BookingDataModel.BookingStateType> BookingState { get; set; }
  }
}
