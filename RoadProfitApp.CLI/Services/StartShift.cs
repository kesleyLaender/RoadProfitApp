using System;
using System.Text.Json;

namespace RoadProfitApp.CLI.Services;

internal class ShiftServices
{
    public static void OpenShift()
    {
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
            string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dataFolder = Path.Combine(baseDirectory, "RoadProfitData");
            string path = Path.Combine(dataFolder, "opened_shift.json");

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            if (!File.Exists(path))
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
            }
            else
            {
                Console.WriteLine("Já existe uma jornada aberta. Encerre-a antes de iniciar outra.");
            }
            return;

        }
        else
        {
            Console.WriteLine("Operação cancelada.");
            return;
        }
    }
}
