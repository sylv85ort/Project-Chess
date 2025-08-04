using System.Diagnostics;
using ChessBackend;
using Microsoft.AspNetCore.Mvc;
using static ChessBackend.ChessPiece;

[ApiController]
[Route("api/[controller]")]
public class ChessController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly GameEngine _gameEngine;

    public ChessController(GameService gameService, GameEngine gameEngine)
    {
        _gameService = gameService;
        _gameEngine = gameEngine;
    }

    [HttpPost("create-game")]
    public IActionResult StartGame([FromBody] CreateGameRequest request)
    {
        try
        {
            int newGameId = _gameService.StartNewGame(request.Player1Id, request.Player2Id);

            var initialBoard = _gameEngine.GetInitialBoardState();
            _gameService.SaveBoardToDatabase(newGameId, initialBoard);

            var (whiteId, blackId) = _gameService.GetPlayerColors(newGameId);

            return Ok(new GameResponse
            {
                GameId = newGameId,
                WhitePlayerId = whiteId,
                BlackPlayerId = blackId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create game", detail = ex.Message });
        }
    }
    [HttpPost("replay-game")]
    public IActionResult StartReplay([FromBody] CreateReplayRequest request)
    {
        try
        {
            var initialBoard = _gameEngine.GetInitialBoardState();
            this.GetSnapshots(request.GameId);
            return Ok(request
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to replay game", detail = ex.Message });
        }
    }

    [HttpGet("game-details")]
    public IActionResult GetDetails([FromQuery] int gameId)
    {
        var (whiteId, blackId) = _gameService.GetPlayerColors(gameId);

        return Ok(new GameResponse
        {
            GameId = gameId,
            WhitePlayerId = whiteId,
            BlackPlayerId = blackId
        });        
    }

    [HttpGet("GetBoard")]
    public IActionResult GetBoard([FromQuery] int gameId)
    {
        try
        {
            var boardState = _gameService.LoadGame(gameId);
            return Ok(boardState);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to load game", detail = ex.Message });
        }
    }
    [HttpGet ("GetSnapshots")]
    public IActionResult GetSnapshots(int gameId)
    {
        var snapshots = _gameService.GetSnapshots(gameId);

        if (snapshots == null | snapshots.Count == 0)
        {
            return NotFound("No game of this ID available");
        }
        return Ok(snapshots);
    }

    [HttpPost("MovePiece")]
    public IActionResult MovePiece([FromBody] MoveRequest move)
    {
        var dbPieces = _gameService.LoadGame(move.GameId);
        var board = _gameEngine.BuildBoard(dbPieces);

        var piece = board[move.From.X, move.From.Y];
        if (piece == null)
            return Ok(new { validMove = false, message = "No piece at selected position" });

        string actualColor = piece.Color.ToString();
        int playerId = _gameService.GetPlayerIdByColor(move.GameId, actualColor);
        int userId = _gameService.GetUserIdByGamePlayer(playerId);

        Debug.WriteLine($"Move requested by User {move.UserId}, piece color: {actualColor}, DB says user: {userId}");


        if (userId != move.UserId)
            return Ok(new { validMove = false, message = "Unauthorized move attempt" });

        var result = _gameEngine.ValidateAndMovePiece(move.From, move.To, move.PieceType, piece.Color, board);

        if (result.IsValid)
        {
            var updatedBoardState = _gameEngine.ConvertBoardToState(board);
            _gameService.SaveBoardToDatabase(move.GameId, updatedBoardState);
            _gameService.SaveSnapshot(move.GameId, move.turnNumber, updatedBoardState);

            return Ok(new { validMove = true, newPosition = result.NewPosition });
        }

        return Ok(new { validMove = false, message = result.Message });
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = new[]
        {
            new { userId = 1, username = "Adam" },
            new { userId = 2, username = "Eve" },
            new { userId = 3, username = "Matthew" },
            new { userId = 4, username = "Paul" },
            new { userId = 5, username = "John" }
        };

        return Ok(users);
    }
}

public class CreateGameRequest
{
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }
}

public class CreateReplayRequest
{
    public int GameId { get; set; }
}

public class MoveRequest
{
    public Coord From { get; set; }
    public Coord To { get; set; }
    public PieceType PieceType { get; set; }
    public PieceColor PieceColor { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int turnNumber { get; set; }
}

public class GameResponse
{
    public int GameId { get; set; }
    public int WhitePlayerId { get; set; }
    public int BlackPlayerId { get; set; }
}
