namespace RoadProfitApp.CLI.Common;

public static class AppPaths
{
    public static readonly string BaseDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RoadProfitData");
    public static readonly string ActiveShiftPath = Path.Combine(BaseDataDir, "active_shift.json");
    public static readonly string AppStatePath = Path.Combine(BaseDataDir, "state.json");
    public static readonly string ShiftsPath = Path.Combine(BaseDataDir, "shifts.jsonl");
    public static readonly string LogFilePath = Path.Combine(BaseDataDir, "logs.jsonl");

    static AppPaths() => Directory.CreateDirectory(BaseDataDir); 
}
