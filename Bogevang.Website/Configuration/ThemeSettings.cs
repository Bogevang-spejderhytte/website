using Cofoundry.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Website.Configuration
{
    public class ThemeSettings : IConfigurationSettings
    {
        [Required]
        public string SiteTitle { get; set; }
        
        [Required]
        public string SiteSubTitle { get; set; }
    }
}