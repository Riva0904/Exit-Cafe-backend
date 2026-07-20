using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExitCafe.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static readonly string[] RoleNames = { "SuperAdmin", "Admin", "Manager", "Staff", "Customer" };

    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, IConfiguration configuration)
    {
        await context.Database.MigrateAsync();

        foreach (var roleName in RoleNames)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == roleName))
                context.Roles.Add(new Role { Name = roleName, Description = $"{roleName} role" });
        }
        await context.SaveChangesAsync();

        var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@exitcaff.com";
        var adminPassword = configuration["SeedAdmin:Password"] ?? "ChangeMe123!";

        if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var superAdminRole = await context.Roles.FirstAsync(r => r.Name == "SuperAdmin");
            context.Users.Add(new User
            {
                Email = adminEmail,
                PasswordHash = passwordHasher.Hash(adminPassword),
                FirstName = "Exit",
                LastName = "Admin",
                RoleId = superAdminRole.Id,
                IsActive = true,
                EmailConfirmed = true
            });
            await context.SaveChangesAsync();
        }
    }
}
