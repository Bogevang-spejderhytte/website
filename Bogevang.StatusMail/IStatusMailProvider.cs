using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.StatusMail.Domain
{
  public interface IStatusMailProvider
  {
    Task<string> BuildStatusCalendar(DateTime startDate, DateTime endDate);
    Task<IDictionary<string, object>> BuildStatusCalendarContent(DateTime startDate, DateTime endDate);
  }
}
