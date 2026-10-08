using System;
using System.Text.Json;

namespace RoadProfitApp.CLI.Services;

public partial class ShiftServices
{
    public static void OpenShift()
    {
        string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dataFolder = Path.Combine(baseDirectory, "RoadProfitData");
        string path = Path.Combine(dataFolder, "opened_shift.json");

        if (File.Exists(path))
        {
            Console.WriteLine("\nJá existe um turno aberto, operação cancelada.");
            return;
        }

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        var now = DateTime.Now;
        int startOdometer = 0;

        while (true)
        {
            Console.Write("informe o odometro atual: ");
            string? odometerInputed = Console.ReadLine();

            if (!string.IsNullOrEmpty(odometerInputed) && int.TryParse(odometerInputed, out startOdometer) && startOdometer > 0)
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
                var openedShift = new
                {
                    date = now.ToString("yyyy-MM-dd"),
                    startOdometer = startOdometer,
                    startTime = now.ToString("HH:mm:ss")
                };

                string jsonString = JsonSerializer.Serialize(openedShift);
                File.WriteAllText(path, jsonString);

                Console.WriteLine("Inicio da jornada registrado com sucesso!");

            return;

        }
        else
        {
            Console.WriteLine("Operação cancelada.");
            return;
        }
    }
}
