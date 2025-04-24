namespace CatalogApi.Logging
{
	public class CustomLogger(string name, CustomLoggerProviderConfiguration loggerConfig) : ILogger
	{
		private readonly string _name = name;

		private readonly CustomLoggerProviderConfiguration _loggerConfig = loggerConfig;

		public bool IsEnabled(LogLevel logLevel) => logLevel == _loggerConfig.LogLevel;

		public IDisposable BeginScope<TState>(TState state) => null;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			string message = $"{logLevel}: {eventId.Id} - {formatter(state, exception)}";
		}
	}
}