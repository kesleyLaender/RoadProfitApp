using RoadProfitApp.CLI.Application;
using RoadProfitApp.CLI.Infrastructure;
using RoadProfitApp.CLI.Common;

namespace RoadProfitApp.CLI;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = new FileLogger();
        var repo = new LocalStorage(logger);
        var shiftService = new ShiftServices(repo);

        WriteLine("===========RoadProfit=============");
        WriteLine("1- Abrir turno");
        WriteLine("2- Encerrar turno");
        WriteLine("3- Registrar abastecimento");
        WriteLine("4- Registrar manutenção");
        Write("\nInforme a opção: ");
        string option = ReadLine()?.Trim() ?? string.Empty;

        switch (option)
        {
            case "1"

                var startOdometer = GetValidInt("Informe o odometro Atual.");

                var retroResult = GetTimeStamp();
                if (retroResult.IsFail) { WriteLine(retroResult.Error.Message); break; }
                var startTime = retroResult.Value;

                var request = new OpenShiftRequest(startOdometer, startTime);
                while (true)
                {
                    var response = shiftService.Open(request);
                    if (response is FailureResponse) { WriteLine(response.Message); break; } 

                    if (response is WarningResponse)
                    {
                        WriteLine(response.Message);
                        request = request with { ConfirmWarnings = true };
                        continue; 
                    }
                    WriteLine(response.Message);
                    break;

                }

        }
    }
}
/*
            case "2":
                shiftService.CloseShift();
                return;
        }



    }

}
       
   









    
