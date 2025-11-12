using GenericRepository;
using RentCarServer.Domain.Users;
using RentCarServer.Domain.Users.ValueObjects;

namespace WebAPI;

public static class ExtensionMethods
{
    public static async Task CreateFirstUser(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        if (!(await userRepository.AnyAsync(p => p.UserName.Value == "admin")))
        {
            FirstName firstName = new("Erdem");
            LastName lastName = new("Kaya");
            Email email = new("erdem.kaya@de-kaya.com");
            UserName userName = new("admin");
            Password password = new("Admin123");

            var user = new User(firstName, lastName, email, userName, password);

            userRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
