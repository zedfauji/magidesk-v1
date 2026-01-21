using Magidesk.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Magidesk.Domain.Tests.Entities;

public class PromotionScheduleTests
{
    private readonly Guid _discountId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldCreateValidSchedule()
    {
        var startTime = new TimeSpan(17, 0, 0); // 5 PM
        var endTime = new TimeSpan(19, 0, 0);   // 7 PM
        
        var schedule = PromotionSchedule.Create(_discountId, DayOfWeek.Friday, startTime, endTime);

        schedule.DiscountId.Should().Be(_discountId);
        schedule.DayOfWeek.Should().Be(DayOfWeek.Friday);
        schedule.StartTime.Should().Be(startTime);
        schedule.EndTime.Should().Be(endTime);
        schedule.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldThrow_WhenEndTimeBeforeStartTime()
    {
        var startTime = new TimeSpan(19, 0, 0);
        var endTime = new TimeSpan(17, 0, 0);

        Action act = () => PromotionSchedule.Create(_discountId, DayOfWeek.Friday, startTime, endTime);

        act.Should().Throw<Magidesk.Domain.Exceptions.BusinessRuleViolationException>();
    }

    [Fact]
    public void IsApplicable_ShouldReturnTrue_WhenTimeIsWithinWindow()
    {
        var startTime = new TimeSpan(17, 0, 0);
        var endTime = new TimeSpan(19, 0, 0);
        var schedule = PromotionSchedule.Create(_discountId, DayOfWeek.Friday, startTime, endTime);

        // Friday 6 PM
        var checkTime = new DateTime(2023, 1, 6, 18, 0, 0); // Jan 6 2023 was a Friday

        schedule.IsApplicable(checkTime).Should().BeTrue();
    }

    [Fact]
    public void IsApplicable_ShouldReturnFalse_WhenDayIsDifferent()
    {
        var startTime = new TimeSpan(17, 0, 0);
        var endTime = new TimeSpan(19, 0, 0);
        var schedule = PromotionSchedule.Create(_discountId, DayOfWeek.Friday, startTime, endTime);

        // Saturday 6 PM
        var checkTime = new DateTime(2023, 1, 7, 18, 0, 0);

        schedule.IsApplicable(checkTime).Should().BeFalse();
    }

    [Fact]
    public void IsApplicable_ShouldReturnFalse_WhenTimeIsOutsideWindow()
    {
        var startTime = new TimeSpan(17, 0, 0);
        var endTime = new TimeSpan(19, 0, 0);
        var schedule = PromotionSchedule.Create(_discountId, DayOfWeek.Friday, startTime, endTime);

        // Friday 4 PM
        var checkTime = new DateTime(2023, 1, 6, 16, 0, 0);

        schedule.IsApplicable(checkTime).Should().BeFalse();
    }
}
