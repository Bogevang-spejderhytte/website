using Cofoundry.Domain;
using System.Collections.Generic;

namespace Bogevang.Menus.ViewComponents
{
  public class NestedMenuItemViewModel
  {
    public string Title { get; set; }

    public PageRoute PageRoute { get; set; }

    public ICollection<NestedMenuChildItemViewModel> ChildItems { get; set; }
  }
}
