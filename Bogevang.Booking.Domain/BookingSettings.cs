using Cofoundry.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Bogevang.Booking.Domain
{
  public class BookingSettings : IConfigurationSettings
  {
    [Required]
    public decimal StandardDeposit { get; set; }
    
    public decimal ElectricityPrice { get; set; }

    public string BankAccount { get; set; }
  }
}
