using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using NancyAPI.Core;
using NancyAPI.Utils;

namespace NancyAPI
{
    public class Startup
    {
        public IConfiguration AppConfiguration { get; set; }

        public Startup(IConfiguration config)
        {
            AppConfiguration = config;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Config.Key = AppConfiguration["key"];
            Config.HomeSection = AppConfiguration["home_section"];
            Config.UrlTemplate = AppConfiguration["url_template"];

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug();
            ILogger logger = loggerFactory.CreateLogger<Startup>();
            Logger.Configure(logger);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new Bootstrapper()));
        }
    }
}
