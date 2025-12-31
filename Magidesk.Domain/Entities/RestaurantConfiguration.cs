using System.ComponentModel.DataAnnotations;

namespace Magidesk.Domain.Entities;

public class RestaurantConfiguration
{
    [Key]
    public int Id { get; set; } // Singleton: always 1

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "My Restaurant";

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Website { get; set; } = string.Empty;

    // Legal / Receipt Footer info
    [MaxLength(500)]
    public string ReceiptFooterMessage { get; set; } = "Thank you for dining with us!";
    
    [MaxLength(50)]
    public string TaxId { get; set; } = string.Empty;

    public bool IsKioskMode { get; set; } = false;
}
