using Microsoft.Extensions.Logging;

namespace Catalog.Core.Logging;

public class CustomLogger : ILogger
{
    private readonly string _loggerName;
    private readonly CustomLoggerProviderConfiguration _loggerConfig;
    public static bool LogFile { get; set; } = false;
    
    public CustomLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig)
    {
        _loggerName = loggerName;
        _loggerConfig = loggerConfig;
    }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = $"Execution log { logLevel }: {eventId} - {formatter(state, exception)}";

        if (LogFile)
            WriteLogMessageInFile(message);
        else
            Console.WriteLine(message);
    }

    private void WriteLogMessageInFile(string message)
    {
        var filePath = Environment.CurrentDirectory + $@"loggin\LOG-{DateTime.Now:yyyy-MM-dd}.txt";
        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.Create(filePath).Dispose();
        }

        using StreamWriter sw = new(filePath, true);
        sw.WriteLine(message);
        sw.Close();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}