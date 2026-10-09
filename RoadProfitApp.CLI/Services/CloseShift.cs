using RoadProfitApp.CLI.Models;
using System;
using static System.Globalization.CultureInfo;
using System.Text.Json;
using System.Runtime.CompilerServices;
namespace RoadProfitApp.CLI.Services;

public partial class ShiftServices
{
    public void CloseShift()
    {
        if (!File.Exists(_shiftFilePath))
        {
            Console.WriteLine("Não há turno em aberto. Operação cancelada");
            return;
        }

        string jsonString = File.ReadAllText(_shiftFilePath);
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

            if(!string.IsNullOrEmpty(revenueInputed) && decimal.TryParse(revenueInputed, InvariantCulture, out revenue) && revenue > 0)
            {
                break;
            }
            Console.WriteLine("Valor inválido! Formato esperado: 150.00");
        }

        WorkShift shift = new(startDateTime: openedShift!.startDateTime,
                                  endDateTime: now,
                                  startOdometer: openedShift.startOdometer,
                                  endOdometer: endOdometer,
                                  revenue: revenue);


        string filePath = Path.Combine(dataFolder, "shifts.csv");
    }
    public record OpenedShift(DateTime startDateTime, int startOdometer);
}
