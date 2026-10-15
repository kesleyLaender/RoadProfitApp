using System;

namespace RoadProfitApp.CLI
{
    public class FilePaths
    {
        private static readonly string _baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string DataFolderPath = Path.Combine(_baseDirectory, "RoadProfitData");

        public static readonly string OpenedShiftPath = Path.Combine(DataFolderPath, "opened_shift.json");
        public static readonly string AppStatePath = Path.Combine(DataFolderPath, "app_state.json");
        public static readonly string ShiftsRepoPath = Path.Combine(DataFolderPath, "shifts.csv");
    }
}
