namespace RoadProfitApp.CLI.Domain;

    public record ActiveShift(DateTime StartedAt, int StartOdometer);
    public record AppState(int LastKnownOdometer, DateTime UpdatedAt);
