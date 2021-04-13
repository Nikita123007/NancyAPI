using Nancy;
using Nancy.TinyIoc;
using NancyAPI.Core.Services;

namespace NancyAPI.Utils
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        public Bootstrapper()
        {
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            
            container.Register<IArticlesSourceService, ArticlesSourceService>();
            container.Register<IArticlesService, ArticlesService>();
        }
    }
}
