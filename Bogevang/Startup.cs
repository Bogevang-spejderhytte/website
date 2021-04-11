using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Bogevang.Website
{
  public class Startup
  {
    public IWebHostEnvironment WebHostEnvironment { get; set; }
    public IConfiguration Configuration { get; }


    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
      WebHostEnvironment = env;
      Configuration = configuration;
    }

    
    public void ConfigureServices(IServiceCollection services)
    {
      services
          .AddDataProtection()
          .PersistKeysToFileSystem(new DirectoryInfo($@"{WebHostEnvironment.ContentRootPath}\Keys"))
          .SetApplicationName("Bogevang");

      services
          .AddControllersWithViews()
          .AddCofoundry(Configuration);
    }

    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (!env.IsDevelopment())
      {
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseCofoundry();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "bookingDocuments",
          pattern: "booking-documents/{documentId}",
          defaults: new { controller = "BookingDocument", action = "Document" });
        
        endpoints.MapControllerRoute(
          name: "adminBookings",
          pattern: "admin-bookings/{action}",
          defaults: new { controller = "AdminBookings" });
      });
    }
  }
}
