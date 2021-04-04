using Cofoundry.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Menus.CustomEntities
{
  public class NestedMenuDataModel : ICustomEntityDataModel
  {
    [Required]
    [NestedDataModelCollection(IsOrderable = true)]
    public ICollection<NestedMenuItemDataModel> Items { get; set; }
  }
}
