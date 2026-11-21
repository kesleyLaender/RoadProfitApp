using RoadProfitApp.CLI.Infrastructure;
using RoadProfitApp.CLI.Domain;
namespace RoadProfitApp.CLI.Application;

public class ShiftServices(LocalStorage repo)
{
    public Result OpenShift(OpenShiftRequest request)
    {
        if (repo.HasActiveShift())
            return Result.Fail(Operational("Já existe um turno em aberto!"));

        var occurredAtResult = DomainPolicies.ResolveOccurredAt(
            request.MinutesPassed,
            request.RegisteredAt,
            request.Force);

        if (occurredAtResult.IsFailed)
            return occurredAtResult;

        if (HasAppState() is false)
            return Result.Fail(Confirm("Não foi possível encontrar o cache de estado."));

        var stateResult = repo.GetState();
        if (stateResult.IsFailed)
            return stateResult;

        var odometerValidation = DomainPolicies.ValidateOdometer(
            stateResult.GetValue(),
            request.StartOdometer,
            request.Force);

        if(odometerValidation.IsFailed)
            return odometerValidation;

        var shift = new ActiveShift(
            occurredAtResult.GetValue(),
            request.StartOdometer);

        var saveShiftResult = repo.SaveActiveShift(shift);

        if(saveShiftResult.IsFailed)
            return saveShiftResult;

        var newState = new AppState(
            request.StartOdometer,
            request.RegisteredAt);

        var updateStateResult = repo.UpdateState(newState);

        if(updateStateResult.IsFailed)
            return Result.Fail(Warning(
                "Turno aberto e salvo, mas estado não pôde ser atualizado",
                updateStateResult.GetError().Message));

        return Result.Ok();

    }
}
