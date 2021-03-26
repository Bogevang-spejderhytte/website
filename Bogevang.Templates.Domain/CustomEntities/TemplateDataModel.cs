using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Templates.Domain.CustomEntities
{
  public class TemplateDataModel : ICustomEntityDataModel
  {
    [Display(Name = "Beskrivelse", Description = "Beskrivelse af skabelonen.")]
    public string Description { get; set; }


    [Display(Name = "Emnefelt", Description = "Emnefelt til mails.")]
    public string Subject { get; set; }


    [Display(Name = "Skabelontekst", Description = "Skabelonens indhold. Brug $xxx$ for flettefelter.")]
    [Required]
    [Html(HtmlToolbarPreset.Headings, HtmlToolbarPreset.AdvancedFormatting, HtmlToolbarPreset.Source)]
    public string Text { get; set; }
  }
}
