using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ramone;

namespace Bogevang.StatusMail.Domain
{
  public class EloverblikAPI : IEloverblikAPI
  {
    private EloverblikApiSettings Settings { get; set; }


    public EloverblikAPI(EloverblikApiSettings settings)
    {
      Settings = settings;
    }


    class StandardResponse<T>
    {
      public T[] result { get; set; }
    }

    public class MyEnergyData_MarketDocumentResult
    {
      public MyEnergyData_MarketDocument MyEnergyData_MarketDocument { get; set; }
    }

    public class MyEnergyData_MarketDocument
    {
      public TimeSeries[] TimeSeries { get; set; }
    }

    public class TimeSeries
    {
      public PeriodEntry[] Period { get; set; }
    }

    public class PeriodEntry
    {
      public TimeInterval timeInterval { get; set; }
      public Point[] Point { get; set; }
    }

    public class Point
    {
      [JsonProperty("out_Quantity.quantity")]
      public string out_Quantity_quantity { get; set; }
    }

    public class TimeInterval
    {
      public DateTime start { get; set; }
      public DateTime end { get; set; }
    }


    public async Task<TimeSeries> LoadElectricityTimeSeries(DateTime start, DateTime end)
    {
      IService elService = RamoneConfiguration.NewService(new Uri(Settings.BaseUrl));
      ISession elSession = elService.NewSession();

      var tokenRequest = elSession
        .Bind("api/token")
        .Header("Authorization", "BEARER " + Settings.AccessToken);

      using (var tokenResponse = await tokenRequest.Async().Get())
      {
        dynamic body = tokenResponse.Body;
        string accessToken = body.result;

        // Cannot ask for data in future
        if (end >= DateTime.Now.Date)
          end = DateTime.Now.Date.AddDays(-1);

        var args = new
        {
          dateFrom = start.ToString("yyyy-MM-dd"),
          dateTo = end.ToString("yyyy-MM-dd")
        };
        var request = elSession
          .Bind("api/MeterData/GetTimeSeries/{dateFrom}/{dateTo}/Day", args)
          .Header("Authorization", "BEARER " + accessToken)
          .AsJson()
          .AcceptJson();

        TimeZoneInfo dkZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        using (var response = await request.Async().Post<StandardResponse<MyEnergyData_MarketDocumentResult>>(new { meteringPoints = new { meteringPoint = new string[] { "571313174111455847" } } }))
        {
          var data = response.Body;

          if (data != null && data.result != null && data.result.Length > 0 && data.result[0] != null
            && data.result[0].MyEnergyData_MarketDocument != null
            && data.result[0].MyEnergyData_MarketDocument.TimeSeries != null
            && data.result[0].MyEnergyData_MarketDocument.TimeSeries.Length > 0)
          {
            var timeSeries = data.result[0].MyEnergyData_MarketDocument.TimeSeries[0];

            foreach (var entry in timeSeries.Period)
            {
              if (entry?.timeInterval?.start != null)
                entry.timeInterval.start = TimeZoneInfo.ConvertTimeFromUtc(entry.timeInterval.start, dkZone);

              if (entry?.timeInterval?.end != null)
                entry.timeInterval.end = TimeZoneInfo.ConvertTimeFromUtc(entry.timeInterval.end, dkZone);
            }

            return timeSeries;
          }
          else
            return null;
        }
      }
    }
  }
}
