using RoadProfitApp.CLI.Domain;
using System.Runtime.CompilerServices;

namespace RoadProfitApp.CLI.Infrastructure;

public class LocalStorage(FileLogger logger)
{
    private static readonly string BaseDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RoadProfitData");
    private static readonly string ActiveShiftPath = Path.Combine(BaseDataDir, "active_shift.json");
    private static readonly string AppStatePath = Path.Combine(BaseDataDir, "state.json");
    private static readonly string ShiftsPath = Path.Combine(BaseDataDir, "shifts.jsonl");

    public bool HasActiveShift() 
        => File.Exists(AppPaths.ActiveShiftPath);

    public Result<AppState> GetState()
        => LoadJson<AppState>(AppPaths.AppStatePath);

    public Result UpdateState(AppState state)
        => WriteJson<AppState>(AppPaths.BaseDataDir, AppPaths.AppStatePath, state);

    public Result SaveActiveShift(ActiveShift shift)
        => WriteJson<ActiveShift>(AppPaths.BaseDataDir, AppPaths.ActiveShiftPath, shift);

    public Result<ActiveShift> GetActiveShift()
        => LoadJson<ActiveShift>(AppPaths.ActiveShiftPath);


    // Leitor genérico Json temporario
    private Result<T> LoadJson<T>(string path, [CallerMemberName] string context = "")
    {
        string jsonString = string.Empty;
        try
        {
            if (!File.Exists(path))
                return Result.Fail(
                    Operational($"Arquivo {path} não encontrado"));

            jsonString = File.ReadAllText(path);
            T? obj = JsonSerializer.Deserialize<T>(jsonString);

            return obj is null
                ? throw new JsonException(
                    "O Arquivo JSON foi lido com sucesso, mas o conteudo desserializado resultou em nulo.")
                : Result.Ok(obj);
        }
        catch (IOException ex)
            { return Result.Fail(
                Operational(ex.Message)); }

        catch (JsonException ex) 
        {
            logger.LogError(context, ex, jsonString);
            return Result.Fail(
                Critical(ex.Message)); 
        }
    }

    // Escritor generico Json temporario
    private Result WriteJson<T>(string baseDirectory, string path, T obj, [CallerMemberName] string context = "")
    {
        string fileTemp = Path.Combine(baseDirectory, Path.GetRandomFileName());
        try
        {
            string jsonString = JsonSerializer.Serialize(obj);
            File.WriteAllText(fileTemp, jsonString);
            File.Move(fileTemp, path, overwrite: true);

            return Result.Ok();
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            logger.LogError(context, ex, obj);
            return Result.Fail(
                Critical(ex.Message)); 
        }

        catch (IOException ex)
            { return Result.Fail(
                Operational(ex.Message)); }

        finally { File.Delete(fileTemp); }
    }

    private Result AppendJsonl<T>(string path, T obj, [CallerMemberName] string context = "")
    {

        try
        {
            string line = JsonSerializer.Serialize(obj);
            File.AppendAllText(path, line);
            return Result.Ok();
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            logger.LogError(context, ex, obj);
            return Result.Fail(
                Critical(ex.Message));
        }
        catch (IOException ex)
        {
            return Result.Fail(
                Operational(ex.Message));
        }
    }
}

