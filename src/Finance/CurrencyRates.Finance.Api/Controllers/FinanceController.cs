using CurrencyRates.Common.Authentication;
using CurrencyRates.Finance.Application.Features;
using CurrencyRates.Finance.Application.Features.Favorites;
using CurrencyRates.Finance.Application.Features.GetUserRates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyRates.Finance.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/finance")]
public sealed class FinanceController(ISender sender) : ControllerBase
{
    // курсы валют пользователя
    [HttpGet("rates")]
    public async Task<ActionResult<IReadOnlyList<CurrencyRateDto>>> GetUserRates(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUserRatesQuery(User.GetUserId()), cancellationToken));

    // добавление в избранное
    [HttpPut("favorites/{code}")]
    public async Task<IActionResult> AddFavorite(string code, CancellationToken cancellationToken)
    {
        await sender.Send(new AddFavoriteCurrencyCommand(User.GetUserId(), code), cancellationToken);
        return NoContent();
    }
}
