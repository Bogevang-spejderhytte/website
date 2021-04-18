using Bogevang.Blogging.Domain.Data;
using Cofoundry.Domain;

namespace Bogevang.Blogging.Website.ViewComponents
{
  public class BlogPostListViewModel
  {
    public PagedQueryResult<BlogPostSummary> BlogPosts { get; set; }
  }
}
