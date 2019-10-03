using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace FileLogger
{
	internal class FileLoggerOptionsSetup : ConfigureFromConfigurationOptions<FileLoggerOptions>
	{
		/// <summary>
		/// Constructor that takes the IConfiguration instance to bind against.
		/// </summary>
		public FileLoggerOptionsSetup(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
			: base(providerConfiguration.Configuration)
		{
		}
	}
}
