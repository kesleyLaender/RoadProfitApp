using RoadProfitApp.CLI.Domain;

namespace RoadProfitApp.CLI.Application;

public sealed record OpenShiftRequest
{
    public DateTime OccurredAt { get; }
    public int StartOdometer {  get; }
    public DateTime RegisteredAt { get; }
    public bool IsConfirmed {  get; }

    private OpenShiftRequest(
        DateTime occurredAt,
        int startOdometer,
        DateTime registeredAt,
        bool isConfirmed )
    {
        OccurredAt = occurredAt;
        StartOdometer = startOdometer;
        RegisteredAt = registeredAt;
        IsConfirmed = isConfirmed;
    }

    public static Result<OpenShiftRequest> Create(
        int minutesAgo,
        int startOdometer,
        DateTime registeredAt,
        bool isConfirmed = false)
    {
        if (startOdometer <= 0)
            return Result.Fail(Validation(
                "Odometro deve ser maior que zero."));

        var timeResult = DomainPolicies.ResolveEventTime(
            minutesAgo,
            registeredAt,
            isConfirmed);

        if (timeResult.IsFailed)
            return Result.Fail(timeResult.GetError());

        var occurredAt = timeResult.GetValue();

        var request = new OpenShiftRequest(
            occurredAt,
            startOdometer,
            registeredAt,
            isConfirmed);

        return Result.Ok(request);
    }

}


public sealed record CloseShiftRequest
{
    public DateTime OccurredAt { get; }
    public int EndOdometer { get; }
    public decimal Revenue { get; }
    public DateTime RegisteredAt { get; }
    public bool IsConfirmed { get; }

    private CloseShiftRequest(
        DateTime occurredAt,
        int endOdometer,
        decimal revenue,
        DateTime registeredAt,
        bool isConfirmed)
    {
        OccurredAt = occurredAt;
        EndOdometer = endOdometer;
        Revenue = revenue;
        RegisteredAt = registeredAt;
        IsConfirmed = isConfirmed;
    }

    public static Result<CloseShiftRequest> Create(
        int minutesAgo,
        int endOdometer,
        decimal revenue,
        DateTime registeredAt,
        bool isConfirmed = false)
    {
        const decimal MaxRevenue = 1000m;

        if (revenue <= 0)
            return Result.Fail(Validation(
                "O faturamento deve ser maior que zero."));

        if (revenue >= MaxRevenue && isConfirmed is false)
            return Result.Fail(Confirmation(
                $"Faturamento suspeito de {revenue}."));

        if (endOdometer <= 0)
            return Result.Fail(Validation(
                $"Odometro deve ser maior que zero."));

        var timeResult = DomainPolicies.ResolveEventTime(
            minutesAgo,
            registeredAt,
            isConfirmed);

        if (timeResult.IsFailed) 
            return Result.Fail(timeResult.GetError());

        var occurredAt = timeResult.GetValue();


        var request = new CloseShiftRequest(
            occurredAt,
            endOdometer,
            revenue,
            registeredAt,
            isConfirmed);

        return Result.Ok(request);
    }
}

