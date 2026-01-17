using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BaseApiController : ControllerBase
{
    protected Guid UserId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    protected string UserRole =>
        User.FindFirstValue(ClaimTypes.Role)!;
    protected string Jti =>
        User.FindFirstValue(JwtRegisteredClaimNames.Jti)!;
}
