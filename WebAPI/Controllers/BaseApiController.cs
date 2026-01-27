using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BaseApiController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (!Guid.TryParse(sub, out var userId))
                throw new UnauthorizedAccessException("UserId claim is missing or invalid.");
            
            return userId;
        }
    }
    protected string Jti
    {
        get
        {
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrWhiteSpace(jti))
                throw new UnauthorizedAccessException("JTI claim is missing.");
            return jti;
        }
    }
    // ApiResponse ni bitta standart bilan qaytarish
    protected IActionResult FromApiResponse<T>(ApiResponse<T> response)
        => StatusCode(response.StatusCode, response);
}
