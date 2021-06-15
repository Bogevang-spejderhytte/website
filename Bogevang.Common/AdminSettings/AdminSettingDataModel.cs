using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Bogevang.Common.AdminSettings
{
  public class AdminSettingDataModel : ICustomEntityDataModel
  {
    [Required]
    [Display(Name = "Værdi")]
    public string Value { get; set; }
  }
}
