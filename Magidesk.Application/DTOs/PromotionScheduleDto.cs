namespace Magidesk.Application.DTOs;

public class PromotionScheduleDto
{
    public Guid Id { get; set; }
    public Guid DiscountId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }

    public string DayName => DayOfWeek.ToString();
    public string TimeWindow => $"{DateTime.Today.Add(StartTime):t} - {DateTime.Today.Add(EndTime):t}";
}
