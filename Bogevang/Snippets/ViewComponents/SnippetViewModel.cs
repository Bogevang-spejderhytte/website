using Microsoft.AspNetCore.Html;

namespace Bogevang.Snippets.ViewComponents
{
  public class SnippetViewModel
  {
    public string SnippetName { get; set; }
    public IHtmlContent Content { get; set; }
  }
}
