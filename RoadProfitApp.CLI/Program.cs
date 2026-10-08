using System;
using RoadProfitApp.CLI.Services;

namespace RoadProfitApp.CLI;

public class Program
{
    public static void Main(string[] args)
    {

        Console.WriteLine("===========RoadProfit=============");
        Console.WriteLine("1- Abrir turno");
        Console.WriteLine("2- Encerrar turno");
        Console.WriteLine("3- Registrar abastecimento");
        Console.WriteLine("4- Registrar manutenção");
        Console.Write("\nInforme a opção: ");
        string? option = Console.ReadLine();

        switch (option)
        {
            case "1":
                ShiftServices.OpenShift();
                return;
        }



    }

}
       
   









    
