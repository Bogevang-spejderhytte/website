using Cofoundry.Domain;
using System.Threading.Tasks;

namespace Bogevang.Blogging.Domain.Data
{
  public class BlogPostPageDisplayModelMapper :
    ICustomEntityDisplayModelMapper<BlogPostDataModel, BlogPostPageDisplayModel>
  {
    public Task<BlogPostPageDisplayModel> MapDisplayModelAsync(CustomEntityRenderDetails entity, BlogPostDataModel dataModel, PublishStatusQuery publishStatusQuery)
    {
      var displayModel = new BlogPostPageDisplayModel();

      displayModel.PageTitle = entity.Title;
      displayModel.MetaDescription = dataModel.ShortDescription;

      displayModel.ShortDescription = dataModel.ShortDescription;

      return Task.FromResult(displayModel);
    }
  }
}
