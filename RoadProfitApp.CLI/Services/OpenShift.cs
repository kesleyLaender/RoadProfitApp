using System;
using System.Text.Json;

namespace RoadProfitApp.CLI.Services;

public partial class ShiftServices
{
    static ShiftServices()
    {
            Directory.CreateDirectory(FilePaths.DataFolderPath);
    }

    public static void OpenShift()
    {
        if (File.Exists(FilePaths.OpenedShiftPath))
        {
            Console.WriteLine("\nJá existe um turno aberto, operação cancelada.");
            return;
        }
        AppState? appState = null;
        if(File.Exists(FilePaths.AppStatePath))
        {
            try
            {
                string json = File.ReadAllText(FilePaths.AppStatePath);
                appState = JsonSerializer.Deserialize<AppState>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Aviso] Não foi possivel encontrar o odometro atual. {ex.Message}");
            }
        }

        int lastOdometer = appState?.Odometer ?? 0;
        var now = DateTime.Now;
        int startOdometer;

        while (true)
        {
            Console.Write("informe o odometro atual: ");
            string? odometerInputed = Console.ReadLine();

            if (!string.IsNullOrEmpty(odometerInputed) && int.TryParse(odometerInputed, out startOdometer) && startOdometer >= lastOdometer)
            {
                break;
            }
            Console.WriteLine("Valor inválido, utilize apenas números inteiros maiores que zero.");
            
        }

        Console.WriteLine("====== Dados Informados ======");
        Console.WriteLine($"Data: {now:dd-MM-yyyy}");
        Console.WriteLine($"Odometro: {startOdometer}");
        Console.WriteLine($"Hora: {now:HH:mm:ss}");
        Console.Write("\nConfirma os dados? [S/N]: ");
        string? input = Console.ReadLine()?.Trim().ToUpper();

        //registrar inicio turno

        if (input == "S")
        {

            string jsonString = string.Empty;

            OpenedShift openedShift = new(
                    StartDateTime: now,
                    StartOdometer: startOdometer
                );

            try
            { 
                jsonString = JsonSerializer.Serialize(openedShift);
                File.WriteAllText(FilePaths.OpenedShiftPath, jsonString);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO]{ex.Message}");
                Console.WriteLine("Os dados não salvos a seguir(anote): ");
                Console.WriteLine(jsonString);
                return;
            }


            try
            {
                var appStateAt = new AppState(openedShift.StartOdometer, now);
                string appStateJson = JsonSerializer.Serialize(appStateAt);
                File.WriteAllText(FilePaths.AppStatePath, appStateJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Aviso] Não foi possivel atualizar last_odometer.json. {ex.Message}");
            }

            Console.WriteLine("Inicio da jornada registrado com sucesso!");

            Console.WriteLine("app_state.json atualizado com sucesso");

            return;
        }
        else
        {
            Console.WriteLine("Operação cancelada.");
            return;
        }
    }
}
