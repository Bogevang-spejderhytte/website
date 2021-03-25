using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Templates.Domain.CustomEntities
{
  public class TemplateDataModel : ICustomEntityDataModel
  {
    [Display(Name = "Skabelontekst", Description = "Skabelonens indhold. Brug $xxx$ for flettefelter.")]
    [Required]
    [Html(HtmlToolbarPreset.Headings, HtmlToolbarPreset.AdvancedFormatting, HtmlToolbarPreset.Source)]
    public string Text { get; set; }
  }
}
