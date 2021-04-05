using Bogevang.Snippets.CustomEntities;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Snippets.ViewComponents
{
  public class SnippetViewComponent : ViewComponent
  {
    private readonly IContentRepository ContentRepository;


    public SnippetViewComponent(
      IContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }


    public async Task<IViewComponentResult> InvokeAsync(string name)
    {
      var snippetEntity = await GetSnippetByIdAsync(name);

      // If not exists, return empty model
      if (snippetEntity == null)
        return View(new SnippetViewModel());

      var snippetModel = (SnippetDataModel)snippetEntity.Model;

      var viewModel = new SnippetViewModel
      {
        Content = new HtmlString(snippetModel.Content)
      };

      return View(viewModel);
    }


    private async Task<CustomEntityRenderSummary> GetSnippetByIdAsync(string name)
    {
      var customEntityQuery = new GetCustomEntityRenderSummariesByUrlSlugQuery(SnippetCustomEntityDefinition.DefinitionCode, name);
      var snippets = await ContentRepository.ExecuteQueryAsync(customEntityQuery);

      // Forcing UrlSlug uniqueness is a setting on the custom entity definition and therefpre
      // the query has to account for multiple return items. Here we only expect one item.
      return snippets.FirstOrDefault();
    }
  }
}
