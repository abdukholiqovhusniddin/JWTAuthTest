using System.Security.Claims;
using Domain.Enums;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, UserRole role);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string requestAccessToken);
}
