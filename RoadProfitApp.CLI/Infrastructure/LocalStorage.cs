using RoadProfitApp.CLI.Domain;
using System.Runtime.CompilerServices;

namespace RoadProfitApp.CLI.Infrastructure;

public class LocalStorage(AppPaths paths, FileLogger logger)
{
    private readonly JsonSerializerOptions _writeOptions = new()
    { WriteIndented = true };

    public bool HasActiveShift() 
        => File.Exists(paths.ActiveShiftPath);

    public Result<AppState> GetState()
        => LoadJson<AppState>(paths.AppStatePath);

    public Result Save(AppState stateResult)
        => WriteJsonAtomic<AppState>(paths.BaseDataDir, paths.AppStatePath, stateResult);

    public Result Save(ActiveShift shift)
        => WriteJsonAtomic<ActiveShift>(paths.BaseDataDir, paths.ActiveShiftPath, shift);

    public Result<ActiveShift> GetActiveShift()
        => LoadJson<ActiveShift>(paths.ActiveShiftPath);

    public Result Save(WorkShift shift)
        => AppendJsonLine<WorkShift>(paths.ShiftsPath, shift);

    public ErrorOr<Success> DeleteActiveShift()
    {
        try
        {
            File.Delete(paths.ActiveShiftPath);
            return Result.Success; 
        }
        catch (IOException ex)
        { return Error.Operational(ex.Message); }
    }

    public ErrorOr<int> GetLastKnownOdometer(int currentOdometer)
    {
        if(File.Exists(paths.AppStatePath) is false)
            return currentOdometer;

        var stateResult = LoadJson(paths.AppStatePath);
        if (stateResult.IsFail)
            return stateResult.Error;

        return stateResult.Value.LastKnownOdometer;
    }

    // Leitor genérico Json temporario
    private ErrorOr<TValue> LoadJson<TValue>(string path, [CallerMemberName] string context = "")
    {
        string jsonString = string.Empty;
        try
        {
            jsonString = File.ReadAllText(path);
            T? obj = JsonSerializer.Deserialize<T>(jsonString);

            return obj is null
                ? throw new JsonException(
                    "O Arquivo JSON foi lido com sucesso, mas o conteudo desserializado resultou em nulo.")
                : obj;
        }
        catch (IOException ex)
            { return Error.Operational(ex.Message); }

        catch (JsonException ex) 
        {
            logger.LogError(context, ex, jsonString);
            return Error.Critical(ex.Message); 
        }
    }

    // Escritor generico Json temporario
    private ErrorOr<Success> WriteJsonAtomic<TValue>(string baseDirectory, string path, TValue obj, [CallerMemberName] string context = "")
    {
        string fileTemp = Path.Combine(baseDirectory, Path.GetRandomFileName());
        try
        {
            string jsonString = JsonSerializer.Serialize(obj, _writeOptions);
            File.WriteAllText(fileTemp, jsonString);
            File.Move(fileTemp, path, overwrite: true);

            return Result.Success;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            logger.LogError(context, ex, obj);
            return Error.Critical(ex.Message); 
        }

        catch (IOException ex)
            { return Error.Operational(ex.Message); }

        finally { File.Delete(fileTemp); }
    }

    private ErrorOr<Success> AppendJsonLine<TValue>(string path, TValue obj, [CallerMemberName] string context = "")
    {

        try
        {
            string line = JsonSerializer.Serialize(obj);
            File.AppendAllText(path, line + Environment.NewLine);
            return Result.Success;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            logger.LogError(context, ex, obj);
            return Error.Critical(ex.Message);
        }
        catch (IOException ex)
        { return Error.Operational(ex.Message); }
    }
}

