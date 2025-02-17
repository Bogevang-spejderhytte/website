﻿using Bogevang.Menus.CustomEntities;
using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Menus.ViewComponents
{
  /// <summary>
  /// Here we use a view component to load the menu data
  /// from Cofoundry repositories, map it into a view model
  /// and then return a view for rendering.
  /// </summary>
  public class NestedMenuViewComponent : ViewComponent
  {
    private readonly IContentRepository ContentRepository;

    
    public NestedMenuViewComponent(
        IContentRepository contentRepository)
    {
      ContentRepository = contentRepository;
    }

    
    public async Task<IViewComponentResult> InvokeAsync(string menuId)
    {
      var viewModel = new NestedMenuViewModel();
      viewModel.MenuId = menuId;

      // Get the menu entity
      var menuEntity = await GetMenuByIdAsync(menuId);

      // If not exists, return empty model
      if (menuEntity == null)
        return View(viewModel);

      var dataModel = (NestedMenuDataModel)menuEntity.Model;

      // Gather all pages required for mapping
      var allPageIds = ExtractPageIds(dataModel);
      var allPages = await ContentRepository
          .Pages()
          .GetByIdRange(allPageIds)
          .AsRoutes()
          .ExecuteAsync();

      // Map the menu items
      viewModel.Items = EnumerableHelper.Enumerate(dataModel.Items)
          .Select(i => MapNestedItemViewModel(i, allPages))
          .Where(i => i.PageRoute != null)
          .ToArray();

      // If you wanted different menu styles you could pass the 
      // menuId here as the view name e.g. "main" or "footer" menu
      return View(viewModel);
    }

    private NestedMenuItemViewModel MapNestedItemViewModel(NestedMenuItemDataModel dataModel, IReadOnlyDictionary<int, PageRoute> allPages)
    {
      var viewModelItem = new NestedMenuItemViewModel()
      {
        Title = dataModel.Title,
        PageRoute = allPages.GetValueOrDefault(dataModel.PageId)
      };

      viewModelItem.ChildItems = EnumerableHelper
          .Enumerate(dataModel.ChildItems)
          .Select(i => MapNestedChildItemViewModel(i, allPages))
          .Where(i => i.PageRoute != null)
          .ToArray();

      return viewModelItem;
    }

    private NestedMenuChildItemViewModel MapNestedChildItemViewModel(NestedMenuChildItemDataModel dataModel, IReadOnlyDictionary<int, PageRoute> allPages)
    {
      var viewModelItem = new NestedMenuChildItemViewModel()
      {
        Title = dataModel.Title,
        PageRoute = allPages.GetValueOrDefault(dataModel.PageId)
      };

      return viewModelItem;
    }

    private static IEnumerable<int> ExtractPageIds(NestedMenuDataModel dataModel)
    {
      var level1Itds = dataModel
          .Items
          .Select(i => i.PageId);

      var level2Ids = dataModel
          .Items
          .Where(i => i.ChildItems != null)
          .SelectMany(i => i.ChildItems)
          .Select(ci => ci.PageId);

      return level1Itds.Union(level2Ids);
    }

    private async Task<CustomEntityRenderSummary> GetMenuByIdAsync(string menuId)
    {
      var customEntityQuery = new GetCustomEntityRenderSummariesByUrlSlugQuery(NestedMenuCustomEntityDefinition.DefinitionCode, menuId);
      var menus = await ContentRepository.ExecuteQueryAsync(customEntityQuery);

      // Forcing UrlSlug uniqueness is a setting on the custom entity definition and therefpre
      // the query has to account for multiple return items. Here we only expect one item.
      return menus.FirstOrDefault();
    }
  }
}
