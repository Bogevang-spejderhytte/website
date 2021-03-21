using Bogevang.Booking.Domain.TenantCategories.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class BookingRequestCommand : ICommand
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


    [Display(Name = "Lejers navn", Description = "Angiv navn på den organisation som ønsker at leje Bøgevang (behøver ikke at være en formel organisation).")]
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
  }
}
