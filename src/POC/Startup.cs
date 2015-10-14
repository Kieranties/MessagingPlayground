using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using POC.Messaging;
using System;

namespace POC
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;
        private readonly IConfigurationSection _queueConfig;
        private readonly Options _options;

        public Startup(IApplicationEnvironment appEnv, ILoggerFactory loggerFactory, IHostingEnvironment hostEnv)
        {
            _appEnv = appEnv;

            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole(loggerFactory.MinimumLevel);
            
            var runtimeConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables() // currently not accessible from command line
                .Build();            

            _options = new Options();
            runtimeConfig.Bind(_options);

            _queueConfig = new ConfigurationBuilder(_appEnv.ApplicationBasePath)
               .AddJsonFile("queues.json")
               .Build()
               .GetSection(_options.QueueType);
        }
        
        public void ConfigureServices(IServiceCollection services)
        {   
            services.AddMvc();
            services.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();

            switch (_options.QueueType)
            {
                case "zeromq":
                    services.AddZeroMq(_queueConfig);
                    break;
                case "msmq":
                    services.AddMsmq(_queueConfig);
                    break;
                case "azure":
                    services.AddAzure(_queueConfig);
                    break;
                default:
                    throw new Exception($"Could not resolve queue type {_options.QueueType}");
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseErrorPage();
            app.UseFileServer();

            app.UseMvc();            
        }

        private class Options<T> where T : IMessageQueueConnection
        {
            public string QueueType { get; set; }
        }
    }
}
