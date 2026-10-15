using RoadProfitApp.CLI.Models;
using System;
using static System.Globalization.CultureInfo;
using System.Text.Json;
using static System.FormattableString;
namespace RoadProfitApp.CLI.Services;

public partial class ShiftServices
{
    public static void CloseShift()
    {
        if (!File.Exists(FilePaths.OpenedShiftPath))
        {
            Console.WriteLine("Não há turno em aberto. Operação cancelada");
            return;
        }
        var now = DateTime.Now;
        string jsonString = string.Empty;
        OpenedShift? openedShift = null;
        try
        {
            jsonString = File.ReadAllText(FilePaths.OpenedShiftPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Erro]{ex.Message}");
            Console.WriteLine("Intervenção manual necessaria.");
            Console.WriteLine($"Registro da falha: {now}");
            return;
        }
        try
        { 
            openedShift = JsonSerializer.Deserialize<OpenedShift>(jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO]{ex.Message}");
            Console.WriteLine("Registro manual necessario, segue dados capturados: ");
            Console.WriteLine(jsonString);
            Console.WriteLine($"Resgistro da falha: {now}");
            Console.WriteLine("==========================================");
            Console.WriteLine("[CONFIRA O ARQUIVO MANUALMENTE E EXCLUA!!] ");
            return;
        }

        int endOdometer;
        decimal revenue;

        while (true)
        {
            Console.Write("\nInforme o odômetro atual: ");
            string? odometerInputed = Console.ReadLine();

            if (!string.IsNullOrEmpty(odometerInputed) &&
                int.TryParse(odometerInputed, out endOdometer) &&
                endOdometer > 0)
            {
                break;
            }
            Console.WriteLine("Valor inválido, utilize apenas números inteiros maiores que zero.");
        }
        
        while (true)
        {
            Console.Write("\nInforme o faturamento: ");
            string? revenueInputed = Console.ReadLine();

            if(!string.IsNullOrEmpty(revenueInputed) && 
                decimal.TryParse(revenueInputed, InvariantCulture, out revenue) &&
                revenue > 0)
            {
                break;
            }
            Console.WriteLine("Valor inválido! Formato esperado: 150.00");
        }

        WorkShift shift = new(startDateTime: openedShift!.StartDateTime,
                                  endDateTime: now,
                                  startOdometer: openedShift.StartOdometer,
                                  endOdometer: endOdometer,
                                  revenue: revenue);


        if (!File.Exists(FilePaths.ShiftsRepoPath))
        {

            string csvHeader = $"StartDateTime,EndDateTime,StartOdometer,EndOdometer,Revenue" + Environment.NewLine;
            File.WriteAllText(FilePaths.ShiftsRepoPath, csvHeader);

        }
        
        string csvLine = Invariant(
            $"{shift.StartDateTime},{shift.EndDateTime},{shift.StartOdometer},{endOdometer},{revenue}") + Environment.NewLine;

        try
        {
            File.AppendAllText(FilePaths.ShiftsRepoPath, csvLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO]{ex.Message}");
            Console.WriteLine("Os dados não salvos a seguir(anote): ");
            Console.WriteLine(csvLine);
            Console.WriteLine("====================================");
            Console.WriteLine("APÓS O REGISTRO EXCLUA O ARQUIVO TEMP");
            return;
        }

        Console.WriteLine("Jornada salva com sucesso.");

        try
        {
            var appStateAt = new AppState(shift.EndOdometer, now);
            string appStateJson = JsonSerializer.Serialize<AppState>(appStateAt);
            File.WriteAllText(FilePaths.AppStatePath, appStateJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Aviso] Não foi possivel atualizar last_odometer.json. {ex.Message}");
        }

        Console.WriteLine("last_odometer.json atualizado com sucesso");

        File.Delete(FilePaths.OpenedShiftPath);
        Console.WriteLine("opened_shift.json deletado com sucesso.");
        
        return;

    }
    public record OpenedShift(DateTime StartDateTime, int StartOdometer);
    public record AppState(int Odometer, DateTime DateTime);
}
