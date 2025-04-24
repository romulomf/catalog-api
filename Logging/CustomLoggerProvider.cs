using System.Collections.Concurrent;

namespace CatalogApi.Logging
{
	public class CustomLoggerProvider(CustomLoggerProviderConfiguration loggerConfig) : ILoggerProvider
	{
		private readonly CustomLoggerProviderConfiguration _loggerConfig = loggerConfig;

		private readonly ConcurrentDictionary<string, CustomLogger> _loggers = [];

		public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new CustomLogger(name, _loggerConfig));

		public void Dispose() => _loggers.Clear();
	}
}