using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Bogevang.Common.Utility
{
  //ModelExpressionProvider modelExpressionProvider,

  public static class CustomEntityListEditorExtension
  {
    public static IHtmlContent CustomEntityDropDownListFor(
      this IHtmlHelper<CustomEntityList> html,
      CustomEntityList list,
      Expression<Func<CustomEntityList, CustomEntityList>> expression)
    {
      var items = from entity in list.Entities
                  select new SelectListItem
                  {
                    Text = entity.Value,
                    Value = entity.Key.ToString(),
                    //Selected = enumValue.Equals(metadata.Model)
                  };

      return html.DropDownListFor(expression, items, string.Empty, null);
    }
  }
}
