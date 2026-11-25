using RoadProfitApp.CLI.Domain;
using System.Runtime.CompilerServices;

namespace RoadProfitApp.CLI.Infrastructure;

public class LocalStorage(AppPaths paths, FileLogger logger)
{
    private readonly JsonSerializerOptions _writeOptions = new()
    { WriteIndented = true };

    public bool HasActiveShift() 
        => File.Exists(paths.ActiveShiftPath);

    public bool HasAppState()
        => File.Exists(paths.AppStatePath);

    public Result<AppState> GetState()
        => LoadJson<AppState>(paths.AppStatePath);

    public Result Save(AppState state)
        => WriteJsonAtomic<AppState>(paths.BaseDataDir, paths.AppStatePath, state);

    public Result Save(ActiveShift shift)
        => WriteJsonAtomic<ActiveShift>(paths.BaseDataDir, paths.ActiveShiftPath, shift);

    public Result<ActiveShift> GetActiveShift()
        => LoadJson<ActiveShift>(paths.ActiveShiftPath);

    public Result Save(WorkShift shift)
        => AppendJsonLine<WorkShift>(paths.ShiftsPath, shift);

    public Result DeleteActiveShift()
    {
        try
        {
            File.Delete(paths.ActiveShiftPath);
            return Result.Ok(); 
        }
        catch (IOException ex)
        {
            return Result.Fail(Operational(ex.Message));
        }
    }
    // Leitor genérico Json temporario
    private Result<T> LoadJson<T>(string path, [CallerMemberName] string context = "")
    {
        string jsonString = string.Empty;
        try
        {
<<<<<<< HEAD
            if (!File.Exists(path))
                return Result.Ok(null);

=======
>>>>>>> b8629dfc4b5771822a7007a0218af22e4a9d18b3
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
    private Result WriteJsonAtomic<T>(string baseDirectory, string path, T obj, [CallerMemberName] string context = "")
    {
        string fileTemp = Path.Combine(baseDirectory, Path.GetRandomFileName());
        try
        {
            string jsonString = JsonSerializer.Serialize(obj, _writeOptions);
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

    private Result AppendJsonLine<T>(string path, T obj, [CallerMemberName] string context = "")
    {

        try
        {
            string line = JsonSerializer.Serialize(obj);
            File.AppendAllText(path, line + Environment.NewLine);
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

