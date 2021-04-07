using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.CustomEntities
{
  public enum WeekdayType
  {
    [Description("Mandag")]
    Mon = 1,

    [Description("Tirsdag")]
    Tue = 2,

    [Description("Onsdag")]
    Wed = 3,

    [Description("Torsdag")]
    Thu = 4,

    [Description("Fredag")]
    Fri = 5,

    [Description("Lørdag")]
    Sat = 6,

    [Description("Søndag")]
    Sun = 7
  }


  public class BookingDataModel : ICustomEntityDataModel
  {
    public enum BookingStateType {
      [Description("Forespørgsel")]
      Requested,

      [Description("Godkendt")]
      Approved,

      [Description("Afsluttet")]
      Closed
    }


    [Display(Name = "Aftalenummer")]
    public int BookingNumber { get; set; }


    [Display(Name = "Ankomstdato")]
    [Required]
    [Date]
    public DateTime? ArrivalDate { get; set; }

    [Display(Name = "Afrejsedato")]
    [Required]
    [Date]
    public DateTime? DepartureDate { get; set; }

    [Display(Name = "Kun udvalgte ugedage")]
    public bool OnlySelectedWeekdays { get; set; }

    [Display(Name = "Valgte ugedage")]
    [CheckboxList(typeof(WeekdayType))]
    public ICollection<WeekdayType> SelectedWeekdays { get; set; }


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
    public decimal? RentalPrice { get; set; }


    [Display(Name = "Depositum")]
    public decimal? Deposit { get; set; }


    [Display(Name = "Bookingstatus")]
    [SelectList(typeof(BookingStateType))]
    [Required]
    public BookingStateType? BookingState { get; set; }


    [Display(Name = "Reservation godkendt")]
    public bool IsApproved { get; set; }


    [Display(Name = "Reservation afvist")]
    public bool IsRejected { get; set; }


    [Display(Name = "Velkomstbrev er sendt")]
    public bool WelcomeLetterIsSent { get; set; }


    [Display(Name = "El-forbrug er indmeldt")]
    public bool IsCheckedOut { get; set; }


    [Display(Name = "Reservationen er arkiveret og anonymiseret")]
    public bool IsArchived { get; set; }


    [Display(Name = "Depositum modtaget")]
    public bool DepositReceived { get; set; }


    [Display(Name = "Betaling modtaget")]
    public bool PaymentReceived { get; set; }


    [Display(Name = "Depositum tilbagebetalt")]
    public bool DepositReturned { get; set; }


    [Display(Name = "Automatisk genereret adgangskode til lejer-selvbetjening")]
    public string TenantSelfServiceToken { get; set; }


    [Display(Name = "Automatisk genereret adgangskode til administrator-selvbetjening")]
    public string AdminSelfServiceToken { get; set; }


    [Display(Name = "Aflæsning af el-måler ved ankomst (kWh)")]
    public decimal? ElectricityReadingStart { get; set; }

    
    [Display(Name = "Aflæsning af el-måler ved afrejse (kWh)")]
    public decimal? ElectricityReadingEnd { get; set; }


    [Display(Name = "El-pris ved afregning (kr/kWh)")]
    public decimal? ElectricityPriceUnit { get; set; }


    public decimal ElectricityPrice => ((ElectricityReadingEnd ?? 0) - (ElectricityReadingStart ?? 0)) * (ElectricityPriceUnit ?? 0);

    public decimal TotalPrice => (Deposit ?? 0) - ElectricityPrice;


    public List<BookingLogEntry> LogEntries { get; set; }

    public List<BookingDocumentEntry> Documents { get; set; }


    public BookingDataModel()
    {
      SelectedWeekdays = new List<WeekdayType>();
      TenantSelfServiceToken = RandomKeyGenerator.GetRandomString(30);
      AdminSelfServiceToken = RandomKeyGenerator.GetRandomString(30);
      LogEntries = new List<BookingLogEntry>();
      Documents = new List<BookingDocumentEntry>();
    }


    public string MakeTitle()
    {
      return $"Reservation nr. {BookingNumber} {ArrivalDate.Value.ToShortDateString()} - {DepartureDate.Value.ToShortDateString()} ({BookingState.GetDescription()})";
    }


    public async Task AddLogEntry(ICurrentUserProvider currentUserProvider, string text)
    {
      var user = await currentUserProvider.GetAsync();
      AddLogEntry(new BookingLogEntry
      {
        Text = text,
        Username = user.User.GetFullName(),
        UserId = user.User.UserId,
        Timestamp = DateTime.Now
      });
    }

    public void AddLogEntry(BookingLogEntry entry)
    {
      if (LogEntries == null)
        LogEntries = new List<BookingLogEntry>();

      LogEntries.Add(entry);
    }


    public void AddDocument(string title, int documentId)
    {
      Documents.Add(new BookingDocumentEntry 
      { 
        CreatedDate = DateTime.Now,
        Title = title, 
        DocumentId = documentId
      });
    }
  }
}
