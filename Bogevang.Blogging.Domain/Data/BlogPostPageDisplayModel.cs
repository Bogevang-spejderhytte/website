using Cofoundry.Domain;

namespace Bogevang.Blogging.Domain.Data
{
  public class BlogPostPageDisplayModel : ICustomEntityPageDisplayModel<BlogPostDataModel>
  {
    public string PageTitle { get; set; }
    public string MetaDescription { get; set; }
    public string ShortDescription { get; set; }
  }
}
