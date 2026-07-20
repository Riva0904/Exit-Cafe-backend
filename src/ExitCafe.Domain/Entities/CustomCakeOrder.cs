using ExitCafe.Domain.Common;
using ExitCafe.Domain.Enums;

namespace ExitCafe.Domain.Entities;

public class CustomCakeOrder : BaseAuditableEntity
{
    public string CustomerName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;

    public string Occasion { get; set; } = default!;
    public string Size { get; set; } = default!;
    public string Flavor { get; set; } = default!;
    public string? Shape { get; set; }
    public string? ThemeColor { get; set; }
    public string? Toppings { get; set; }
    public string? CakeMessage { get; set; }

    public DateOnly DeliveryDate { get; set; }
    public TimeOnly? DeliveryTime { get; set; }
    public decimal? Budget { get; set; }
    public string? Notes { get; set; }
    public string? ReferenceImageUrl { get; set; }

    public CustomCakeOrderStatus Status { get; set; } = CustomCakeOrderStatus.New;
}
