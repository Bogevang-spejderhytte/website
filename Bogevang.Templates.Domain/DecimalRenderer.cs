using Antlr4.StringTemplate;
using System.Globalization;

namespace Bogevang.Templates.Domain
{
  public class DecimalRenderer : IAttributeRenderer
  {
    public string ToString(object obj, string formatString, CultureInfo culture)
    {
      if (obj is decimal d)
        return d.ToString("0.00");
      else
        return obj.ToString();
    }
  }
}
