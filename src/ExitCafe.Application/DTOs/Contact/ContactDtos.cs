namespace ExitCafe.Application.DTOs.Contact;

public record CreateContactMessageRequest(string Name, string Email, string? Phone, string Subject, string Message);

public class ContactMessageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string Subject { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
