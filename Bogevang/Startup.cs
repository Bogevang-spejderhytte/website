using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net;

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

      services
        .AddControllers(o =>
        {
          o.Filters.Add<BogevangApiExceptionFilter>();
        });
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (!env.IsDevelopment())
      {
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseCofoundry();
    }
  }

  class BogevangApiExceptionFilter : ExceptionFilterAttribute
  {
    public override void OnException(ExceptionContext context)
    {
      var details = new ProblemDetails
      {
        Title = context.Exception.Message,
      };
      var result = new ObjectResult(details);
      result.StatusCode = (int)HttpStatusCode.InternalServerError;
      context.Result = result;
      context.ExceptionHandled = true;
    }
  }
}
