using Microsoft.AspNetCore.Http;
using RentCarServer.Application.Service;
using System.Security.Claims;

namespace RentCarServer.Infrastructure.Services;
internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid GetUserId()
    {
        var httpcontext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException("HttpContext is null");
        var claims = httpcontext?.User.Claims;
        string? userId = (claims?.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value) ?? throw new ArgumentNullException("User id is null");
        try
        {
            Guid id = Guid.Parse(userId);
            return id;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("User id is not valid guid", ex);
        }
    }
}
