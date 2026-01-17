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
    public async Task<IActionResult> Refresh(string refreshtocen )
        => Ok(await _mediator.Send(new RefreshTokenCommand(refreshtocen, Jti)));

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
        => Ok(await _mediator.Send(new LogoutCommand(UserId, Jti)));
}
