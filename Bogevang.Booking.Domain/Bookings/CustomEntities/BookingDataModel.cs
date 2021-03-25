﻿using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Bookings.CustomEntities
{
  public class BookingDataModel : ICustomEntityDataModel
  {
    public enum BookingStateType { 
      [Description("Forespørgsel")]
      Requested,

      [Description("Bekræftet")]
      Confirmed,

      [Description("Aflyst")]
      Cancelled
    }


    [Display(Name = "Ankomstdato")]
    [Required]
    [Date]
    public DateTime? ArrivalDate { get; set; }

    [Display(Name = "Afrejsedato")]
    [Required]
    [Date]
    public DateTime? DepartureDate { get; set; }


    [Display(Name = "Lejerkategori")]
    [CustomEntity(TenantCategoryCustomEntityDefinition.DefinitionCode)]
    [Required]
    public int? TenantCategoryId { get; set; }


    [Display(Name = "Lejers navn")]
    public string TenantName { get; set; }


    [Display(Name = "Formål")]
    public string Purpose { get; set; }


    [Display(Name = "Kontaktpersons navn")]
    [Required]
    public string ContactName { get; set; }


    [Display(Name = "Kontaktpersons telefonnummer")]
    public string ContactPhone { get; set; }


    [Display(Name = "Kontaktpersons adresse")]
    public string ContactAddress { get; set; }


    [Display(Name = "Kontaktpersons postnummer og by")]
    public string ContactCity { get; set; }


    [Display(Name = "Kontaktpersons e-mailadresse")]
    [EmailAddress]
    public string ContactEMail { get; set; }


    [Display(Name = "Bemærkninger")]
    [MultiLineText]
    public string Comments { get; set; }


    [Display(Name = "Aftalt pris")]
    public decimal RentalPrice { get; set; }


    [Display(Name = "Bookingstatus")]
    [SelectList(typeof(BookingStateType))]
    [Required]
    public BookingStateType? BookingState { get; set; }


    [Display(Name = "Reservation bekræftet")]
    public bool IsConfirmed { get; set; }


    [Display(Name = "Depositum modtaget")]
    public bool DepositReceived { get; set; }


    [Display(Name = "Betaling modtaget")]
    public bool PaymentReceived { get; set; }


    [Display(Name = "Depositum tilbagebetalt")]
    public bool DepositReturned { get; set; }


    public List<BookingLogEntry> LogEntries { get; set; }


    public void AddLogEntry(BookingLogEntry entry)
    {
      if (LogEntries == null)
        LogEntries = new List<BookingLogEntry>();

      LogEntries.Add(entry);
    }
  }
}
