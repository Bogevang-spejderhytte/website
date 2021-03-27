using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.TenantCategories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Models
{
  public class BookingSummary
  {
    public enum AlertType
    {
      New,
      Key,
      Finalize
    }

    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public string Purpose { get; set; }
    public int TenantCategoryId { get; set; }
    public string TenantName { get; set; }
    public string ContactName { get; set; }
    public string ContactEMail { get; set; }
    public BookingDataModel.BookingStateType BookingState { get; set; }
    public string BookingStateText { get; set; }
    public bool IsApproved { get; set; }


    public List<string> Warnings { get; set; }

    public AlertType? Alert { get; set; }


    public async Task UpdateCalculatedValues(ITenantCategoryProvider tenantCategoryProvider)
    {
      Alert = BookingState == BookingDataModel.BookingStateType.Requested
        ? AlertType.New
        : BookingState == BookingDataModel.BookingStateType.Approved && DateTime.Now > DepartureDate
          ? AlertType.Finalize
          : BookingState == BookingDataModel.BookingStateType.Approved && DateTime.Now > ArrivalDate - TimeSpan.FromDays(10)
            ? AlertType.Key
            : (AlertType?)null;

      Warnings = new List<string>();

      var tenantCategory = await tenantCategoryProvider.GetTenantCategoryById(TenantCategoryId);
      DateTime lastAllowedArrivalDate = CreatedDate.AddMonths(tenantCategory.AllowedBookingFutureMonths);

      if (ArrivalDate >= lastAllowedArrivalDate)
        Warnings.Add($"Bemærk at ankomstdatoen ligger mere end de tilladte {tenantCategory.AllowedBookingFutureMonths} måneder efter at ansøgningen blev oprettet. " +
          $"Datoen er beregnet på baggrund af lejerkategorien.");
    }
  }
}
