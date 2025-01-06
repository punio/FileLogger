using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Test
{
	internal class TestService : IHostedService
	{
		private readonly ILogger _logger;
		private IDisposable _timer;

		public TestService(ILogger<TestService> logger)
		{
			_logger = logger;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => OnTimer());
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		private void OnTimer()
		{
			_logger.LogWarning("Warning");
			_logger.LogInformation("Information");
			_logger.LogCritical("Critical");
			_logger.LogDebug("Debug");
			try
			{
				throw new Exception("Exception");
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error");
			}

			_logger.LogTrace("Trace");

			ThreadPool.SetMaxThreads(100, 100);
			for (var i = 0; i < 100; i++)
			{
				ThreadPool.QueueUserWorkItem(t => StressTest((int)t), i);
			}
			_timer.Dispose();
		}

		private void StressTest(int thread)
		{
			for (var i = 0; i < 50; i++)
			{
				Task.Delay(0).Wait();
				_logger.LogInformation($"\t{thread}\t{i}");
			}
		}
	}
}
