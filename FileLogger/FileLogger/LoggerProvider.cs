using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FileLogger
{
	public abstract class LoggerProvider : IDisposable, ILoggerProvider, ISupportExternalScope
	{
		readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>();
		protected IDisposable SettingsChangeToken;
		IExternalScopeProvider _scopeProvider;

		public virtual void Dispose()
		{
			SettingsChangeToken?.Dispose();
			SettingsChangeToken = null;
		}

		public ILogger CreateLogger(string categoryName)
		{
			return _loggers.GetOrAdd(categoryName, (category) => new Logger(this, category));
		}
		public void SetScopeProvider(IExternalScopeProvider scopeProvider)
		{
			_scopeProvider = scopeProvider;
		}


		public abstract bool IsEnabled(LogLevel logLevel);
		public abstract void WriteLog(LogEntry info);


		internal IExternalScopeProvider ScopeProvider => _scopeProvider ?? (_scopeProvider = new LoggerExternalScopeProvider());

		void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
		{
			_scopeProvider = scopeProvider;
		}

	}
}
