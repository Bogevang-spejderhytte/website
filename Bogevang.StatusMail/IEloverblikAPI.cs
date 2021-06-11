using System;
using System.Threading.Tasks;

namespace Bogevang.StatusMail.Domain
{
  public interface IEloverblikAPI
  {
    Task<EloverblikAPI.TimeSeries> LoadElectricityTimeSeries(DateTime start, DateTime end);
  }
}
