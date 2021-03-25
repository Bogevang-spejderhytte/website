using Bogevang.Templates.Domain.CustomEntities;
using System.Threading.Tasks;

namespace Bogevang.Templates.Domain
{
  public interface ITemplateProvider
  {
    Task<TemplateDataModel> GetTemplateByName(string name);
    string MergeText(string text, object mergeData);
  }
}
