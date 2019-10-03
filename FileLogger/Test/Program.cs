using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FileLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Test
{
	class Program
	{
		public static IServiceProvider ServiceProvider { get; private set; }
		public static IConfiguration Configuration { get; private set; }

		static async Task Main(string[] args)
		{
			await new HostBuilder()
				.ConfigureAppConfiguration((hostContext, configApp) =>
				{
					// Configの追加
					configApp.SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
					configApp.AddCommandLine(args);
					configApp.AddJsonFile("appsettings.json");
				})
				.ConfigureServices(services =>
				{
					services.AddHostedService<TestService>();
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
					logging.ClearProviders();
					logging.AddConsole();
					logging.AddDebug();
					logging.AddFileLogger();
				})
				.RunConsoleAsync();

		}
	}
}
