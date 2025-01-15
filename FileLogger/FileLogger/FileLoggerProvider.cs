using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileLogger
{
	[Microsoft.Extensions.Logging.ProviderAlias("File")]
	public class FileLoggerProvider : LoggerProvider
	{
		internal FileLoggerOptions Settings { get; private set; }

		private bool _writing = false;
		private string _currentFile;
		private IDisposable _output = null;
		private readonly Subject<LogEntry> _logOutput = new Subject<LogEntry>();

		private void Initialize()
		{
			_currentFile = Path.Combine(Settings.OutputFolder, string.Format(Settings.FileName, 0));
			_output = _logOutput.Buffer(TimeSpan.FromSeconds(0.5)).Subscribe(WriteCore);
		}

		public override void Dispose()
		{
			base.Dispose();
			_output?.Dispose();
			_output = null;
		}

		public override bool IsEnabled(LogLevel logLevel)
		{
			return logLevel != LogLevel.None && Settings.LogLevel.Default != LogLevel.None && logLevel >= Settings.LogLevel.Default;
		}

		public override void WriteLog(LogEntry log)
		{
			_logOutput.OnNext(log);
		}

		private void WriteCore(IList<LogEntry> logs)
		{
			if (logs.Count == 0 || _writing) return;    // 高負荷時は捨てる

			_writing = true;

			#region Backup
			try
			{
				var info = new FileInfo(_currentFile);
				if (info.Length > Settings.MaxFileSize)
				{
					System.Diagnostics.Debug.WriteLine($"Switch file. {info.Length} > {Settings.MaxFileSize} bytes");
					var index = Settings.RetainFileCount - 1;
					var fileName = Path.Combine(Settings.OutputFolder, string.Format(Settings.FileName, index));
					File.Delete(fileName);
					for (; index > 0; index--)
					{
						var original = Path.Combine(Settings.OutputFolder, string.Format(Settings.FileName, index - 1));
						if (File.Exists(original)) File.Move(original, fileName);
						fileName = original;
					}
				}
			}
			catch
			{
			}
			#endregion
			try
			{
				using var writer = new StreamWriter(_currentFile, true, Encoding.UTF8);
				foreach (var log in logs)
				{
					var time = Settings.LocalTime ? log.Time.ToLocalTime() : log.Time;
					writer.WriteLine($"[{log.Level.ToString()[0]}] {time:yyyy/MM/dd HH:mm:ss.fff} : {log.Text}");
				}
				writer.Close();
			}
			catch
			{
			}

			_writing = false;
		}

		public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> settings)
			: this(settings.CurrentValue)
		{
			SettingsChangeToken = settings.OnChange(s =>
			{
				this.Settings = s;
				Initialize();
			});
		}

		public FileLoggerProvider(FileLoggerOptions settings)
		{
			this.Settings = settings;
			Initialize();
		}

	}
}
