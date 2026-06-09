using DocEditor.API.Models;

namespace DocEditor.API.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Users.Any()) return;

        context.Users.AddRange(
            new User { Id = 1, Username = "alice", Password = "password1", Email = "alice@example.com" },
            new User { Id = 2, Username = "bob",   Password = "password2", Email = "bob@example.com"   }
        );
        context.SaveChanges();
    }
}
