namespace Magidesk.Application.DTOs;

public class MenuCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public bool IsBeverage { get; set; }
    public string? ButtonColor { get; set; }
}