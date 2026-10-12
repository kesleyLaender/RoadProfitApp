using System;
using RoadProfitApp;

namespace RoadProfitApp.CLI.Models;

public class WorkShift
{
    public Guid Id { get; init; }
    public DateTime StartDateTime { get; init; }
    public int StartOdometer { get; init; }
    public int EndOdometer { get; init; }
    public decimal Revenue { get; init; }
    public DateTime EndDateTime { get; init; }

    //Propriedades Calculadas
    public int KmDriven => EndOdometer - StartOdometer;
    public TimeSpan WorkedHours => EndDateTime - StartDateTime;
    public decimal RevenuePerKm => Revenue / (decimal)KmDriven;
    public decimal RevenuePerHour => Revenue / (decimal)WorkedHours.TotalHours;

    //Construtor
    public WorkShift (
        DateTime startDateTime,
        DateTime endDateTime,
        int startOdometer, 
        int endOdometer, 
        decimal revenue,
        DateTime? currentDate = null,
        Guid? id = null )
    {
        var today = currentDate ?? DateTime.Now;

        if (endOdometer <= startOdometer)
            throw new ArgumentException("Odômetro final deve ser maior que o inicial", nameof(endOdometer));

        if (endDateTime <= startDateTime)
            throw new ArgumentException("A hora final deve ser maior que a inicial.", nameof(endDateTime));

        if (startDateTime > today)
            throw new ArgumentException("A data do registro não pode ser futura", nameof(startDateTime));

        Id = id ?? Guid.NewGuid();
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        StartOdometer = startOdometer;
        EndOdometer = endOdometer;
        Revenue = revenue;

    }

}
