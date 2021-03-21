using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class BookingRequestCommand : ICommand, IValidatableObject
  {
    [Display(Name = "Ankomstdato")]
    [Required]
    public DateTime? ArrivalDate { get; set; }

    [Display(Name = "Afrejsedato")]
    [Required]
    public DateTime? DepartureDate { get; set; }


    [Display(Name = "Lejers oprindelse", Description = "Angiv hvor lejer kommer fra. Oplysningen bruges til statistik, prisberegning og ved Bøgevangs ansøgning om kommunalt tilskud.")]
    [CustomEntity(TenantCategoryCustomEntityDefinition.DefinitionCode)]
    [Required]
    public int? TenantCategoryId { get; set; }


    [Display(Name = "Lejers navn", Description = "Angiv navn på den organisation som ønsker at leje Bøgevang (behøver ikke at være en formel organisation). Kan også bare være \"Privat\".")]
    public string TenantName { get; set; }


    [Display(Name = "Formål")]
    public string Purpose { get; set; }


    [Display(Name = "Kontaktpersons navn")]
    [Required]
    public string ContactName { get; set; }


    [Display(Name = "Kontaktpersons telefonnummer")]
    [Required]
    public string ContactPhone { get; set; }


    [Display(Name = "Kontaktpersons adresse")]
    [Required]
    public string ContactAddress { get; set; }


    [Display(Name = "Kontaktpersons postnummer og by")]
    [Required]
    public string ContactCity { get; set; }


    [Display(Name = "Kontaktpersons e-mailadresse")]
    [EmailAddress]
    [Required]
    public string ContactEMail { get; set; }


    [Display(Name = "Bemærkninger")]
    [MultiLineText]
    public string Comments { get; set; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (ArrivalDate > DepartureDate)
        yield return new ValidationResult("Ankomstdatoen skal være før afrejsedatoen.");

      if (ArrivalDate < DateTime.Today)
        yield return new ValidationResult("Det er ikke muligt at reservere hytten før i dag.");
    }
  }
}
