namespace RoadProfitApp.CLI.Common;

public static class InputHelpers
{
    public static int GetValidInt(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int value) || value <= 0)
            {
                WriteLine("(Valor invalido, utilize apenas numeros inteiros maiores que zero.");
                continue;
            }
            return value;
        }
    }

    public static decimal GetValidDecimal(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = ReadLine()?.Replace(',','.');

            if (!decimal.TryParse(input, InvariantCulture, out decimal value) || value <= 0)
            {
                WriteLine("Valor invalido, informe apenas numeros reais maiores que zero.");
                continue;
            }
            return value;
        }
    }

    public static Result<DateTime> GetTimeStamp(string message = "Informe o tempo retroativo em minutos(ex: 30) ou 0 para cancelar: ")
    {
        while (true)
        {
            Write(message);
            string? input = ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return Result.Ok(DateTime.Now);

            if (!int.TryParse(input, out int minutesPassed) || minutesPassed < 0)
            {
                WriteLine("Entrada invalida!");
                continue;
            }
            if (minutesPassed is 0) return Result.Fail(Confirm());

            if (minutesPassed > 60)
            {
                string confirm;
                while (true)
                {
                    Write($"Confirma {minutesPassed} retroativos? [S/N] ou [0] para cancelar: ");
                    confirm = ReadLine()?.Trim()?.ToUpper() ?? string.Empty;

                    if (confirm is "S" or "N" or "0") break;

                    WriteLine("Opção invalida!");
                }
                if (confirm is "0") return Result.Fail(Confirm());
                if (confirm is "N") continue;
            }
            return Result.Ok(DateTime.Now.AddMinutes(-minutesPassed));
        }
    }
}
