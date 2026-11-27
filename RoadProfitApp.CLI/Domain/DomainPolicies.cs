namespace RoadProfitApp.CLI.Domain;

public static class DomainPolicies
{
    public static ErrorOr<Success> ValidateOdometer(int lastKnownOdometer, int currentOdometer, bool isConfirmed)
    {
        const int MaxOdometerJump = 200;

        if (currentOdometer < lastKnownOdometer)
            return Error.Validation(
                $"Odometro informado é menor que o ultimo conhecido {lastKnownOdometer}");

        int odometerJump = currentOdometer - lastKnownOdometer;

        if (odometerJump > MaxOdometerJump && isConfirmed is false)
            return Error.Confirmation(
                $"Salto suspeito de {odometerJump} km no odometro em relação ao ultimo registrado {lastKnownOdometer}");

        return Result.Success;
    }

    public static ErrorOr<DateTime> ResolveEventTime(int minutesAgo, DateTime now, bool isConfirmed)
    {
        const int MaxRetroactiveMinutes = 60;

        if (minutesAgo is 0)
            return now;

        if (minutesAgo > MaxRetroactiveMinutes && isConfirmed is false)
            return Error.Confirmation(
                $"Tempo retroativo suspeito {minutesAgo}");
        
        return now.AddMinutes(-minutesAgo);
    }
}
