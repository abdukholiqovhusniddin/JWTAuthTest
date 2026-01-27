using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers;

public class AuthController(IMediator mediator) : BaseApiController
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand command) 
        => Ok(await _mediator.Send(command));

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(string refreshToken)
    {
        var accessToken = Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "") ?? "";
        return Ok(await _mediator.Send(new RefreshTokenCommand(refreshToken, accessToken)));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand(UserId, Jti));
        return NoContent();
    }
}
