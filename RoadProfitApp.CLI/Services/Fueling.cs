using RoadProfitApp.CLI.Models;
using System;

namespace RoadProfitApp.CLI.Services
{
    public class Fueling(
        DateTime time,
        int odometer,
        decimal totalPrice,
        decimal liters
            )
    {
        public DateTime Time { get; init; } = time;
        public int Odometer { get; init; } = odometer;
        public decimal TotalPrice { get; init; } = totalPrice;
        public decimal Liters { get; init; } = liters;
        public decimal PricePerLiter => TotalPrice / Liters;
    }
}
