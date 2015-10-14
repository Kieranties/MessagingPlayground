using Microsoft.AspNet.Builder;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using POC.Messaging;
using POC.Messaging.MSMQ;
using POC.Messaging.ZeroMq;
using POC.Messaging.Azure;
using System.Collections.Generic;

namespace POC
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IApplicationEnvironment appEnv, ILoggerFactory loggerFactory)
        {
            _appEnv = appEnv;

            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole(loggerFactory.MinimumLevel);
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder(_appEnv.ApplicationBasePath)
                .AddJsonFile($"azure.queues.json")
                .Build();

            var options = new Options();
            config.Bind(options);

            services.AddMvc();
            services.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddSingleton<IMessageQueueFactory>(sp => ActivatorUtilities.CreateInstance<AzureQueueFactory>(sp, options.Queues));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseErrorPage();
            app.UseFileServer();

            app.UseMvc();            
        }

        private class Options
        {
            public Dictionary<string, string> Queues { get; set; } = new Dictionary<string, string>();
        }
    }
}
