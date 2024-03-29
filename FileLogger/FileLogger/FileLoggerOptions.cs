﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FileLogger
{
	public class FileLoggerOptions
	{
		public FileLogLevel LogLevel { get; set; } = new FileLogLevel();

		private string _outputFolder = null;
		public string OutputFolder
		{
			get
			{
				if (string.IsNullOrEmpty(_outputFolder))
				{
					_outputFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? "";
				}
				return _outputFolder;
			}
			set => _outputFolder = value;
		}

		/// <summary>
		/// File name template
		/// {0} : File counter(0:new - )
		/// </summary>
		public string FileName { get; set; } = "{0:d2}.log";

		/// <summary>
		/// Maximum size of single log file.
		/// </summary>
		public int MaxFileSize { get; set; } = 512 * 1024;

		/// <summary>
		/// Maximum number of log files to retain.
		/// </summary>
		public int RetainFileCount { get; set; } = 10;

		/// <summary>
		/// 
		/// </summary>
		public bool LocalTime { get; set; }

		public FileLoggerOptions()
		{
		}
	}

	public class FileLogLevel
	{
		public LogLevel Default { get; set; } = LogLevel.Information;
	}
}
