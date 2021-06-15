using System;
using System.Threading.Tasks;
using Bogevang.StatusMail.Domain;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Bogevang.StatusMail.Web.ViewComponents
{
  public class DashboardViewComponent : ViewComponent
  {
    protected readonly IStatusMailProvider StatusMailProvider;


    public DashboardViewComponent(IStatusMailProvider statusMailProvider)
    {
      StatusMailProvider = statusMailProvider;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
      DateTime startDate = DateTime.Now.AddDays(-14);
      DateTime endDate = DateTime.Now.AddDays(14);
      
      string calendarContent = await StatusMailProvider.BuildStatusCalendar(startDate, endDate);

      return View("Default", new HtmlString(calendarContent));
    }
  }
}
