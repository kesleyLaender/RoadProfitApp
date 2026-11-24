namespace RoadProfitApp.CLI.Infrastructure;

public class AppPaths
{
    public readonly string BaseDataDir;
    public readonly string ActiveShiftPath;
    public readonly string AppStatePath;
    public readonly string ShiftsPath;
    public readonly string LogFilePath;

    public AppPaths(string? customDataDir = null)
    {
        BaseDataDir = customDataDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RoadProfitData");
        ActiveShiftPath = Path.Combine(BaseDataDir, "active_shift.json");
        AppStatePath = Path.Combine(BaseDataDir, "state.json");
        ShiftsPath = Path.Combine(BaseDataDir, "shifts.jsonl");
        LogFilePath = Path.Combine(BaseDataDir, "logs.jsonl");

        Directory.CreateDirectory(BaseDataDir);
    }
}
