using CurrencyRates.Common.Authentication;
using CurrencyRates.Users.Application.Features.Login;
using CurrencyRates.Users.Application.Features.Logout;
using CurrencyRates.Users.Application.Features.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyRates.Users.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    // регистрация
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = await sender.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    // логин
    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenDto>> Login(LoginCommand command, CancellationToken cancellationToken) =>
        await sender.Send(command, cancellationToken);

    // логаут
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await sender.Send(new LogoutCommand(User.GetTokenId(), User.GetTokenExpiration()), cancellationToken);
        return NoContent();
    }
}
