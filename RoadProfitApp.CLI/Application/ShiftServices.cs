using RoadProfitApp.CLI.Infrastructure;
using RoadProfitApp.CLI.Domain;
namespace RoadProfitApp.CLI.Application;

public class ShiftServices(LocalStorage repo)
{
    public Result Open(OpenShiftRequest request)
    {
        if (repo.HasActiveShift())
            return Result.Fail(Operational(
                "Já existe um turno em aberto!"));

        if (repo.HasAppState() is false && request.IsConfirmed is false)
            return Result.Fail(Confirmation(
                "Não foi possível encontrar o cache de estado."));

        var stateResult = repo.GetState();
        if (stateResult.IsFailed)
            return stateResult;

        var odometerValidation = DomainPolicies.ValidateOdometer(
            stateResult.GetValue().LastKnownOdometer,
            request.StartOdometer,
            request.IsConfirmed);

        if(odometerValidation.IsFailed)
            return odometerValidation;

        var shift = new ActiveShift(
            request.OccurredAt,
            request.StartOdometer);

        var saveResult = repo.Save(shift);

        if(saveResult.IsFailed)
            return saveResult;

        var newState = new AppState(
            request.StartOdometer,
            request.RegisteredAt);

        var updateResult = repo.Save(newState);

        if(updateResult.IsFailed)
            return Result.Fail(Warning(
                "Turno aberto e salvo, mas estado não pôde ser atualizado",
                updateResult.GetError().Message));

        return Result.Ok();

    }


    public Result Close(CloseShiftRequest request)
    {
        if (repo.HasActiveShift() is false)
            return Result.Fail(Operational(
                "Não existe um turno ativo para fechar."));

        if (repo.HasAppState() is false && request.IsConfirmed is false)
            return Result.Fail(Confirmation(
                "Arquivo de estado não encontrado."));

        var stateResult = repo.GetState();
        if (stateResult.IsFailed)
            return stateResult;
            
        var odometerValidationResult = DomainPolicies.ValidateOdometer(
            stateResult.GetValue().LastKnownOdometer,
            request.EndOdometer,
            request.IsConfirmed);

        if (odometerValidationResult.IsFailed) 
            return odometerValidationResult;

        var shiftResult = repo.GetActiveShift();
        if (shiftResult.IsFailed)
            return shiftResult;

        var workShift = new WorkShift(
            shiftResult.GetValue().StartedAt,
            request.OccurredAt,
            shiftResult.GetValue().StartOdometer,
            request.EndOdometer,
            request.Revenue);

        var saveResult = repo.Save(workShift);
        if(saveResult.IsFailed)
            return saveResult;

        var newState = new AppState(
            request.EndOdometer,
            request.RegisteredAt);

        var updateResult = repo.Save(newState);
        if (updateResult.IsFailed)
            return Result.Fail(Warning(
                "Falha ao atualizar estado.",
                updateResult.GetError().Message));

        var deleteResult = repo.DeleteActiveShift();

        if (deleteResult.IsFailed)
            return Result.Fail(Warning(
                "Falha ao deletar turno ativo.",
                deleteResult.GetError().Message));

        return Result.Ok();
    }
}
