using Antlr4.StringTemplate;
using System.Globalization;
using System.Web;

namespace Bogevang.Templates.Domain
{
  public class StringRenderer : IAttributeRenderer
  {
    public string ToString(object obj, string formatString, CultureInfo culture)
    {
      if (obj is string s)
      {
        if (formatString == "html")
          return HttpUtility.HtmlEncode(s);
        else
          return s;
      }
      else
        return obj.ToString();
    }
  }
}
