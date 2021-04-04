using System.Collections.Generic;

namespace Bogevang.Menus.ViewComponents
{
  public class NestedMenuViewModel
  {
    public string MenuId { get; set; }

    public ICollection<NestedMenuItemViewModel> Items { get; set; }
  }
}
