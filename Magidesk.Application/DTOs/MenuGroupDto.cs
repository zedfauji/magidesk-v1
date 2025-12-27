namespace Magidesk.Application.DTOs;

public class MenuGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public string? ButtonColor { get; set; }
}