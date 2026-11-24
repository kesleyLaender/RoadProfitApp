namespace RoadProfitApp.CLI.Infrastructure;
public class FileLogger(AppPaths paths)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public void LogError(string context, Exception exception)
        => WriteEntry(context, exception);
    public void LogError(string context, Exception exception, object? payload)
        => WriteEntry(context, exception, payload);

    private void WriteEntry(string context, Exception ex, object? payload = null)
    {
        try
        {
            var entry = new
            {
                TimeStamp = DateTime.Now, 
                Context = context,
                ExceptionType = ex.GetType().Name,
                Message = ex.Message,
                InnerMessage = ex.InnerException?.Message,
                Payload = payload,
                StackTrace = ex.StackTrace
            };

            string line = JsonSerializer.Serialize(entry, _jsonOptions);
            File.AppendAllText(paths.LogFilePath, line + Environment.NewLine);
        }
        catch { }
    }
}
