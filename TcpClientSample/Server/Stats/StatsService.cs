using System;
using System.Web.Http;
using ActorModel.Infrastructure.Actors;
using Microsoft.Owin.Hosting;
using Owin;

namespace Server.Stats
{
    public class StatsService
    {
        public static MailboxMonitor Monitor { get; set; }

        // This code configures Web API. The StatsService class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            appBuilder.UseWebApi(config);
        }

        public static IDisposable Run(MailboxMonitor monitor)
        {
            Monitor = monitor;

            const string baseAddress = "http://localhost:9000/";
            return WebApp.Start<StatsService>(baseAddress);
        }
    } 
}