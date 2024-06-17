using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  // We should use resources for localization, instead of repeating "Feltet '{0}' skal udfyldes." all over the place.
  // But I could not make it work.
  // For a starter see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#dataannotations-localization


  public class BookingRequestCommand : ICommand, IValidatableObject, IIgnorePermissionCheckHandler
  {
    [Display(Name = "Ankomstdato")]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    public DateTime? ArrivalDate { get; set; }

    [Display(Name = "Afrejsedato")]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    public DateTime? DepartureDate { get; set; }

    [Display(Name = "Jeg ønsker kun udvalgte dage (fx alle tirsdage i perioden)")]
    public bool OnlySelectedWeekdays { get; set; }

    [Display(Name = "Valgte ugedage")]
    [CheckboxList(typeof(WeekdayType))]
    public ICollection<WeekdayType> SelectedWeekdays { get; set; }


    [Display(Name = "Lejers baggrund", Description = "Angiv hvor lejer kommer fra. Oplysningen bruges til statistik, prisberegning og ved Bøgevangs ansøgning om kommunalt tilskud.")]
    [CustomEntity(TenantCategoryCustomEntityDefinition.DefinitionCode)]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    public int? TenantCategoryId { get; set; }


    [Display(Name = "Hvilken del af Bøgevang ønskes?")]
    [RadioList(typeof(BookingLocationType))]
    public BookingLocationType Location { get; set; }


    [Display(Name = "Lejers navn (organisation)", Description = "Angiv navn på den organisation som ønsker at leje Bøgevang. Det behøver ikke at være en formel organisation, men kan også bare være \"Privat\".")]
    [MaxLength(100)]
    public string TenantName { get; set; }


    [Display(Name = "Formål", Description = "Angiv kort hvad formålet med lejemålet er (fx spejdertur, klassearrangement, privat arrangement eller lignende).")]
    [MaxLength(200)]
    public string Purpose { get; set; }


    [Display(Name = "Kontaktpersons navn")]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    [MaxLength(100)]
    public string ContactName { get; set; }


    [Display(Name = "Kontaktpersons telefonnummer")]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    [MaxLength(50)]
    public string ContactPhone { get; set; }


    [Display(Name = "Kontaktpersons adresse")]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    [MaxLength(100)]
    public string ContactAddress { get; set; }


    [Display(Name = "Kontaktpersons postnummer og by")]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    [MaxLength(100)]
    public string ContactCity { get; set; }


    [Display(Name = "Kontaktpersons e-mailadresse")]
    [EmailAddress]
    [Required(ErrorMessage = "Feltet '{0}' skal udfyldes.")]
    [MaxLength(100)]
    public string ContactEMail { get; set; }


    [Display(Name = "Bemærkninger", Description = "Angiv eventuelle bemærkninger til lejemålet her.")]
    [MultiLineText(Rows = 6)]
    [MaxLength(2000)]
    public string Comments { get; set; }


    [Display(Name = @"html:Jeg har læst og accepteret <a href=""/handelsbetingelser"" target=""bogevang-secondary"">handelsbetingelserne</a>.")]
    public bool ApproveTerms { get; set; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (ArrivalDate > DepartureDate)
        yield return new ValidationResult("Ankomstdatoen skal være før afrejsedatoen.");

      if (ArrivalDate < DateTime.Today)
        yield return new ValidationResult("Det er ikke muligt at reservere hytten før i dag.");
    }
  }
}
