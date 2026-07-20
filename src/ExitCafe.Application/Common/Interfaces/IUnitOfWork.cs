using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRepository<Category> Categories { get; }
    IRepository<Product> Products { get; }
    IRepository<ProductImage> ProductImages { get; }
    IRepository<Customer> Customers { get; }
    IRepository<CustomerAddress> CustomerAddresses { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<CustomCakeOrder> CustomCakeOrders { get; }
    IRepository<ContactMessage> ContactMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
