using RentCarServer.Domain.Users;

namespace RentCarServer.Application.Service;
public interface IJwtProvider
{
    string CreateToken(User user);
}
