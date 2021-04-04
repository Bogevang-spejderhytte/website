using Cofoundry.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Menus.CustomEntities
{
  public class NestedMenuItemDataModel : INestedDataModel
  {
    [MaxLength(30)]
    [Required]
    public string Title { get; set; }

    [Required]
    [Page]
    public int PageId { get; set; }

    [Display(Name = "Undermenuer")]
    [NestedDataModelCollection]
    public ICollection<NestedMenuChildItemDataModel> ChildItems { get; set; }
  }
}
