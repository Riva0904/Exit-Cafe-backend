using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new Repository<User>(context);
        Roles = new Repository<Role>(context);
        RefreshTokens = new Repository<RefreshToken>(context);
        Categories = new Repository<Category>(context);
        Products = new Repository<Product>(context);
        ProductImages = new Repository<ProductImage>(context);
        Customers = new Repository<Customer>(context);
        CustomerAddresses = new Repository<CustomerAddress>(context);
        Orders = new Repository<Order>(context);
        OrderItems = new Repository<OrderItem>(context);
        AuditLogs = new Repository<AuditLog>(context);
        CustomCakeOrders = new Repository<CustomCakeOrder>(context);
        ContactMessages = new Repository<ContactMessage>(context);
        Notifications = new Repository<Notification>(context);
    }

    public IRepository<User> Users { get; }
    public IRepository<Role> Roles { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }
    public IRepository<Category> Categories { get; }
    public IRepository<Product> Products { get; }
    public IRepository<ProductImage> ProductImages { get; }
    public IRepository<Customer> Customers { get; }
    public IRepository<CustomerAddress> CustomerAddresses { get; }
    public IRepository<Order> Orders { get; }
    public IRepository<OrderItem> OrderItems { get; }
    public IRepository<AuditLog> AuditLogs { get; }
    public IRepository<CustomCakeOrder> CustomCakeOrders { get; }
    public IRepository<ContactMessage> ContactMessages { get; }
    public IRepository<Notification> Notifications { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
