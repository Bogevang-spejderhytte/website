using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Menus.CustomEntities
{
  public class NestedMenuChildItemDataModel : INestedDataModel
  {
    [Required]
    [MaxLength(30)]
    public string Title { get; set; }

    [Required]
    [Page]
    public int PageId { get; set; }
  }
}
