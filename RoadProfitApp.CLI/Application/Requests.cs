namespace RoadProfitApp.CLI.Application;

public record OpenShiftRequest(
    int StartOdometer,
    DateTime RegisteredAt,
    int MinutesPassed = default,
    bool Force = false);

public record CloseShiftRequest(
    int EndOdometer,
    decimal Revenue,
    DateTime RegisteredAt,
    bool Force = false);
