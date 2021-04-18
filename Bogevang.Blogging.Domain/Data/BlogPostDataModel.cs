using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Blogging.Domain.Data
{
  public class BlogPostDataModel : ICustomEntityDataModel
  {
    [MaxLength(1000)]
    [Required]
    [Display(Name = "Opsummering", Description = "En kort beskrivelse af nyheden til liste-visning.")]
    [MultiLineText]
    public string ShortDescription { get; set; }

    [Image]
    [Display(Name = "Frimærkebillede")]
    public int? ThumbnailImageAssetId { get; set; }
  }
}
