namespace RoadProfitApp.CLI.Domain;

public static class DomainPolicies
{
    public static Result ValidateOdometer(AppState state, int startOdometer, bool force)
    {
        if (startOdometer < state.LastKnownOdometer)
            return Result.Fail(Validation(
                $"Odometro inputado é menor que o ultimo conhecido {state.LastKnownOdometer}"));

        int deltaOdometer = startOdometer - state.LastKnownOdometer;
        if (deltaOdometer > 200 && force is false)
            return Result.Fail(Confirm(
                $"Salto suspeito do odometro em relação ao ultimo registrado {state.LastKnownOdometer}"));

        return Result.Ok();
    }

    public static Result<DateTime> ResolveOccurredAt(int minutesPassed, DateTime registeredAt, bool force = false)
    {
        if (minutesPassed < 0)
            return Result.Fail(Validation("Minutos não podem ser negativos."));

        if (minutesPassed is 0)
            return Result.Ok(registeredAt);

        if (minutesPassed > 60 && force is false)
            return Result.Fail(Confirm($"Tempo retroativo suspeito {minutesPassed}"));
        
        return Result.Ok(registeredAt.AddMinutes(-minutesPassed));
    }
}
