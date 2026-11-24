namespace RoadProfitApp.CLI.Domain;

public static class DomainPolicies
{
    public static Result ValidateOdometer(int lastKnownOdometer, int currentOdometer, bool isConfirmed)
    {
        const int MaxOdometerJump = 200;

        if (currentOdometer < lastKnownOdometer)
            return Result.Fail(Validation(
                $"Odometro informado é menor que o ultimo conhecido {lastKnownOdometer}"));

        int odometerJump = currentOdometer - lastKnownOdometer;

        if (odometerJump > MaxOdometerJump && isConfirmed is false)
            return Result.Fail(Confirmation(
                $"Salto suspeito de {odometerJump} km no odometro em relação ao ultimo registrado {lastKnownOdometer}"));

        return Result.Ok();
    }

    public static Result<DateTime> ResolveEventTime(int minutesAgo, DateTime now, bool isConfirmed)
    {
        const int MaxRetroactiveMinutes = 60;

        if (minutesAgo < 0)
            return Result.Fail(Validation(
                "Minutos não podem ser negativos."));

        if (minutesAgo is 0)
            return Result.Ok(now);

        if (minutesAgo > MaxRetroactiveMinutes && isConfirmed is false)
            return Result.Fail(Confirmation(
                $"Tempo retroativo suspeito {minutesAgo}"));
        
        return Result.Ok(now.AddMinutes(-minutesAgo));
    }
}
