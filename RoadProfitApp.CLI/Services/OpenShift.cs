using System;
using System.Text.Json;

namespace RoadProfitApp.CLI.Services;

public partial class ShiftServices
{
    private static readonly string _baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private static readonly string _dataFolder = Path.Combine(_baseDirectory, "RoadProfitData");
    private static readonly string _jsonFilePath = Path.Combine(_dataFolder, "opened_shift.json");

    static ShiftServices()
    {
            Directory.CreateDirectory(_dataFolder);
    }

    public static void OpenShift()
    {
        if (File.Exists(_jsonFilePath))
        {
            Console.WriteLine("\nJá existe um turno aberto, operação cancelada.");
            return;
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
                    dateTime = now,
                    startOdometer = startOdometer
                };

                string jsonString = JsonSerializer.Serialize(openedShift);
                File.WriteAllText(_jsonFilePath, jsonString);

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
