using UserServiceBusiness.Models;

namespace UserServiceDAL.DataContext;

public class Seeder
{
    public static void SeedData(AppDbContext context)
    {
        try
        {
            Console.WriteLine("Seeding Roles...");
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role() { Name = "Admin", Id = 1 },
                    new Role() { Name = "Landlord", Id = 2 },
                    new Role() { Name = "Tenant", Id = 3 },
                    new Role() { Name = "Guest", Id = 4 }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Roles already seeded");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to seed roles: {e.Message}");
        }

        try
        {
            Console.WriteLine("Seeding Users...");
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User{ Name = "Test", Email = Environment.GetEnvironmentVariable("Test__Email")!, RoleId = 4, ProviderAccountId = "" }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Users already seeded");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to seed users: {e.Message}");
        }
    }
}