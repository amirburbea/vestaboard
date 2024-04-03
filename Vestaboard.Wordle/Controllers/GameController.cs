using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vestaboard.Wordle.Models;
using Vestaboard.Wordle.Services;

namespace Vestaboard.Wordle.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class GameController(IGameService gameService, ILogger<GameController> logger) : ControllerBase
{
    [HttpPost("new")]
    public async Task<ActionResult<GameData>> CreateNewGameAsync([FromBody] bool hardMode = false)
    {
        await gameService.StartNewAsync(hardMode, this.HttpContext.RequestAborted);
        logger.LogInformation("Created new game: #{uuid}", gameService.Data.Uuid);
        return this.Ok(gameService.Data);
    }

    [HttpPost("{uuid}/guess")]
    public async Task<ActionResult<Color[]>> EnterGuessAsync(string uuid, [FromBody] string guess)
    {
        return this.ValidateUuid(uuid) is { } result
            ? result
            : await gameService.AddGuessAsync(guess, this.HttpContext.RequestAborted);
    }

    [HttpGet]
    public ActionResult<GameData> GetGameData() => this.Ok(gameService.Data);

    [HttpGet("{uuid}/solution")]
    public ActionResult GetSolution(string uuid)
    {
        if (this.ValidateUuid(uuid) is { } result)
        {
            return result;
        }
        return gameService.Data.IsOver
            ? new OkObjectResult(gameService.Data.Word) { ContentTypes = { "application/json" } }
            : this.StatusCode(500, "Game is not over.");
    }

    [HttpPost("{uuid}/refresh")]
    public async Task<ActionResult<object?>> RenderAsync(string uuid)
    {
        if (this.ValidateUuid(uuid) is { } result)
        {
            return result;
        }
        await gameService.RenderAsync(this.HttpContext.RequestAborted);
        return this.Ok();
    }

    private ObjectResult? ValidateUuid(string uuid) => gameService.Data.Uuid == uuid ? null : this.StatusCode(500, "Incorrect uuid.");
}
