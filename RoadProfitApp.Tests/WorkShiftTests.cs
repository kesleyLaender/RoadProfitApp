using RoadProfitApp.CLI.Models;
using Xunit;

namespace RoadProfitApp.Tests;

public class WorkShiftTests
{
    [Fact]
    public void Constructor_WhenParametersAreValid_ShouldCreateInstanceWithCorrectCalculatedValues()
    {
        //1. Arrange (Preparação)
        var date = new DateTime(2026,09,29);
        var startTime = date.AddHours(7);
        var endTime = date.AddHours(17);
        int startOdometer = 1000;
        int endOdometer = 1100;
        decimal revenue = 200m;

        //2. Act (Ação)
        var shift = new WorkShift(
            date: date,
            startTime: startTime,
            endTime: endTime,
            startOdometer: startOdometer,
            endOdometer: endOdometer,
            revenue: revenue,
            currentDate: date
            );

        //3. Assert (Verificação)
        Assert.NotEqual(Guid.Empty, shift.Id);
        Assert.Equal(100, shift.KmDriven);
        Assert.Equal(TimeSpan.FromHours(10), shift.WorkedHours);
        Assert.Equal(2.0m, shift.RevenuePerKm);
        Assert.Equal(20.0m, shift.RevenuePerHour);
    }

    [Theory]
    [InlineData(900)]
    [InlineData(1000)]
    public void Constructor_WhenEndOdometerIsLessThanOrEqualToStartOdometer_ShouldThrowException(int incorrectOdometer)
    {
        //Arrange
        var date = new DateTime(2026,09,29);

        //Act 
        var ex = Assert.Throws<ArgumentException>(() =>
        new WorkShift(
            date: date,
            startTime: date.AddHours(7),
            endTime: date.AddHours(17),
            startOdometer: 1000,
            endOdometer: incorrectOdometer,
            revenue: 200m,
            currentDate: date
            )
        );
        //Assert
        Assert.Equal("endOdometer" , ex.ParamName );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Constructor_WhenRevenueIsNegativeOrZero_ShouldThrowException(decimal invalidRevenue)
    {
        //Arrange
        var date = new DateTime(2026, 09, 29);

        //Act
        var ex = Assert.Throws<ArgumentException>(() => 
        new WorkShift(
            date : date,
            startTime : date.AddHours(7),
            endTime : date.AddHours(17),
            startOdometer : 1000,
            endOdometer : 1100,
            revenue : invalidRevenue,
            currentDate : date
            )
        );

        //Assert
        Assert.Equal("revenue", ex.ParamName);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(7)]

    public void Constructor_WhenEndTimeIsLessThanOrEqualToStartTime_ShouldThrowException(int incorrectEndTime)
    {
        var date = new DateTime(2026, 09, 29);
        var startTime = date.AddHours(8);
        var endTime = date.AddHours(incorrectEndTime);

        var ex = Assert.Throws<ArgumentException>(() =>
        new WorkShift(
            date : date,
            startTime : startTime,
            endTime : endTime,
            startOdometer : 1000,
            endOdometer : 1100,
            revenue : 200m,
            currentDate : date
            )
        );

        Assert.Equal("endTime" , ex.ParamName);
    }

    [Fact]

    public void Constructor_WhenDateIsInTheFuture_ShouldThrowException()
    {
        var currentDate = new DateTime(2026, 09, 29);
        var futureDate = currentDate.AddDays(1);

        var ex = Assert.Throws<ArgumentException>(() =>
        new WorkShift(
            date: futureDate,
            startTime: currentDate.AddHours(7),
            endTime: currentDate.AddHours(17),
            startOdometer: 1000,
            endOdometer: 1100,
            revenue: 200m,
            currentDate: currentDate
            )
        );

        Assert.Equal("date" , ex.ParamName);
    }
}
