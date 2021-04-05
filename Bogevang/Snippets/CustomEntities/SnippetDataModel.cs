using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Snippets.CustomEntities
{
  public class SnippetDataModel : ICustomEntityDataModel
  {
    [Required]
    [Html(HtmlToolbarPreset.Headings, HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Source)]
    [Display(Name = "Indhold")]
    public string Content { get; set; }
  }
}
