using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FileLogger
{
	public class LogEntry
	{
		public DateTime Time { get; set; }
		public LogLevel Level { get; set; }
		public EventId EventId { get; set; }
		public string Text { get; set; }
		public Exception Exception { get; set; }
	}
}
