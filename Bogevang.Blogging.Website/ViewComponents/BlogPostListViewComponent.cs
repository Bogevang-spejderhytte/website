using Bogevang.Blogging.Domain.Data;
using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Blogging.Website.ViewComponents
{
  public class BlogPostListViewComponent : ViewComponent
  {
    private readonly IContentRepository _contentRepository;
    private readonly IVisualEditorStateService _visualEditorStateService;

    public BlogPostListViewComponent(
        IContentRepository contentRepository,
        IVisualEditorStateService visualEditorStateService
        )
    {
      _contentRepository = contentRepository;
      _visualEditorStateService = visualEditorStateService;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
      // We can use the current visual editor state (e.g. edit mode, live mode) to
      // determine whether to show unpublished blog posts in the list.
      var visualEditorState = await _visualEditorStateService.GetCurrentAsync();
      var ambientEntityPublishStatusQuery = visualEditorState.GetAmbientEntityPublishStatusQuery();

      var query = new SearchCustomEntityRenderSummariesQuery()
      {
        CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode,
        PageNumber = 1,
        PageSize = 5,
        PublishStatus = ambientEntityPublishStatusQuery
      };

      var entities = await _contentRepository
          .CustomEntities()
          .Search()
          .AsRenderSummaries(query)
          .ExecuteAsync();

      var posts = await MapBlogPostsAsync(entities, ambientEntityPublishStatusQuery);

      var viewModel = new BlogPostListViewModel
      {
        BlogPosts = posts
      };

      return View(viewModel);
    }


    private async Task<PagedQueryResult<BlogPostSummary>> MapBlogPostsAsync(
        PagedQueryResult<CustomEntityRenderSummary> customEntityResult,
        PublishStatusQuery ambientEntityPublishStatusQuery)
    {
      var blogPosts = new List<BlogPostSummary>();

      var imageAssetIds = customEntityResult
          .Items
          .Select(i => (BlogPostDataModel)i.Model)
          .Where(m => m.ThumbnailImageAssetId != null)
          .Select(m => m.ThumbnailImageAssetId.Value)
          .Distinct();

      var imageLookup = await _contentRepository
          .ImageAssets()
          .GetByIdRange(imageAssetIds)
          .AsRenderDetails()
          .ExecuteAsync();

      foreach (var customEntity in customEntityResult.Items)
      {
        var model = (BlogPostDataModel)customEntity.Model;

        var blogPost = new BlogPostSummary();
        blogPost.Title = customEntity.Title;
        blogPost.ShortDescription = model.ShortDescription;
        blogPost.ThumbnailImageAsset = imageLookup.GetOrDefault(model.ThumbnailImageAssetId);
        blogPost.FullPath = customEntity.PageUrls.FirstOrDefault();
        blogPost.PostDate = customEntity.PublishDate;

        blogPosts.Add(blogPost);
      }

      return customEntityResult.ChangeType(blogPosts);
    }
  }
}
