using Microsoft.AspNetCore.Mvc;
using Vestaboard.Wordle.Models;
using Vestaboard.Wordle.Services;

namespace Vestaboard.Wordle.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameController> _logger;

    public GameController(IGameService gameService, ILogger<GameController> logger) => (this._gameService, this._logger) = (gameService, logger);

    [HttpPost("new")]
    public async Task<ActionResult<GameData>> CreateNewGameAsync([FromBody] bool hardMode = false)
    {
        await this._gameService.StartNewAsync(hardMode, this.HttpContext.RequestAborted);
        this._logger.LogInformation("Created new game: #{uuid}", this._gameService.Data.Uuid);
        return this.Ok(this._gameService.Data);
    }

    [HttpPost("{uuid}/guess")]
    public async Task<ActionResult<Color[]>> EnterGuessAsync(string uuid, [FromBody] string guess)
    {
        if (this.ValidateUuid(uuid) is { } result)
        {
            return result;
        }
        return await this._gameService.AddGuessAsync(guess, this.HttpContext.RequestAborted);
    }

    [HttpGet]
    public ActionResult<GameData> GetGameData() => this.Ok(this._gameService.Data);

    [HttpGet("{uuid}/solution")]
    public ActionResult GetSolution(string uuid)
    {
        if (this.ValidateUuid(uuid) is { } result)
        {
            return result;
        }
        if (!this._gameService.Data.IsOver)
        {
            return this.StatusCode(500, "Game is not over.");
        }
        return new OkObjectResult(this._gameService.Data.Word) { ContentTypes = { "application/json" } };
    }

    [HttpPost("{uuid}/refresh")]
    public async Task<ActionResult<object?>> RenderAsync(string uuid)
    {
        if (this.ValidateUuid(uuid) is { } result)
        {
            return result;
        }
        await this._gameService.RenderAsync(this.HttpContext.RequestAborted);
        return this.Ok();
    }

    private ActionResult? ValidateUuid(string uuid) => this._gameService.Data.Uuid == uuid ? null : this.StatusCode(500, "Incorrect uuid.");
}