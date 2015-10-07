using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using ZMQ.Sockets;

namespace ZMQ
{
    public class Program
    {
        private readonly IServiceProvider _services;

        public Program()
        {
            _services = BuildServices();
            ConfigureApplication();
        }

        public void Main(string[] args)
        {
            var options = new Options();
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            config.Bind(options);
                        
            AbstractSocket socket;
            switch (options.SocketType)
            {
                case nameof(Push):
                    socket = _services.GetRequiredService<Push>();
                    break;
                case nameof(Pull):
                    socket = _services.GetRequiredService<Pull>();
                    break;
                case nameof(Request):
                    socket = _services.GetRequiredService<Request>();
                    break;
                case nameof(Response):
                    socket = _services.GetRequiredService<Response>();
                    break;
                default:
                    throw new Exception("Unkown socket type");
            }
            
            socket.Start(options);            

            Console.ReadLine();
        }

        private IServiceProvider BuildServices()
        {
            var services = new ServiceCollection();
            services.AddLogging();                        

            services.AddTransient<Push>();
            services.AddTransient<Pull>();
            services.AddTransient<Request>();
            services.AddTransient<Response>();

            return services.BuildServiceProvider();
        }

        private void ConfigureApplication()
        {
            var loggerFactory = _services.GetRequiredService<ILoggerFactory>();
            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole(loggerFactory.MinimumLevel);
        }
    }
}
