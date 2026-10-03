namespace RoadProfitApp.Console.Models;
using System;

public class WorkShift
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public int StartOdometer { get; init; }
    public int EndOdometer { get; init; }
    public decimal Revenue { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }

    //Propriedades Calculadas
    public int KmDriven => EndOdometer - StartOdometer;
    public TimeSpan WorkedHours => EndTime - StartTime;
    public decimal RevenuePerKm => Revenue / (decimal)KmDriven;
    public decimal RevenuePerHour => Revenue / (decimal)WorkedHours.TotalHours;

    //Construtor
    public WorkShift (
        DateTime date,
        DateTime startTime,
        DateTime endTime,
        int startOdometer, 
        int endOdometer, 
        decimal revenue,
        DateTime? currentDate = null,
        Guid? id = null )
    {
        var today = currentDate ?? DateTime.Today;

        if (endOdometer <= startOdometer)
            throw new ArgumentException("Odômetro final deve ser maior que o inicial", nameof(endOdometer));

        if (revenue <= 0)
            throw new ArgumentException("O faturamento deve ser positivo", nameof(revenue));

        if (endTime <= startTime)
            throw new ArgumentException("A hora final deve ser maior que a inicial.", nameof(endTime));

        if (date > today)
            throw new ArgumentException("A data do registro não pode ser futura", nameof(date));

        Id = id ?? Guid.NewGuid();
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        StartOdometer = startOdometer;
        EndOdometer = endOdometer;
        Revenue = revenue;

    }

}
