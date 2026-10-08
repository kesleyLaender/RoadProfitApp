using RoadProfitApp.CLI.Models;
using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
namespace RoadProfitApp.CLI.Services;

public partial class ShiftServices
{
    public static void CloseShift()
    {
        var inv = CultureInfo.InvariantCulture;
        var ptbr = new CultureInfo("pt-BR");

        string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dataFolder = Path.Combine(baseDirectory, "RoadProfitData");
        string path = Path.Combine(dataFolder, "opened_shift.json");

        if (!File.Exists(path))
        {
            Console.WriteLine("Não há turno em aberto. Operação cancelada");
            return;
        }

        string jsonString = File.ReadAllText(path);
        var openedShift = JsonSerializer.Deserialize<OpenedShift>(jsonString);
        var now = DateTime.Now;
        int endOdometer = 0;
        decimal revenue = 0m;

        while (true)
        {
            Console.Write("\nInforme o odômetro atual: ");
            string? odometerInputed = Console.ReadLine();

            if (!string.IsNullOrEmpty(odometerInputed) && int.TryParse(odometerInputed, out endOdometer) && endOdometer > 0)
            {
                break;
            }
            Console.WriteLine("Valor inválido, utilize apenas números inteiros maiores que zero.");
        }
        
        while (true)
        {
            Console.Write("\nInforme o faturamento: ");
            string? revenueInputed = Console.ReadLine();

            if(!string.IsNullOrEmpty(revenueInputed) && decimal.TryParse(revenueInputed, inv, out revenue) && revenue > 0)
            {
                break;
            }
            Console.WriteLine("Valor inválido! Formato esperado: 150.00");
        }

        WorkShift shift = new(date: openedShift.date,
                                  startTime: openedShift.startTime,
                                  endTime: now,
                                  startOdometer: openedShift.startOdometer,
                                  endOdometer: endOdometer,
                                  revenue: revenue);


    }
    public record OpenedShift(DateTime date, int startOdometer, DateTime startTime);
}
