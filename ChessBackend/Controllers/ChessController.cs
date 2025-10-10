using System.Diagnostics;
using ChessBackend;
using ChessBackend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using static ChessBackend.ChessPiece;


[ApiController]
[Route("api/[controller]")]
public class ChessController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly GameEngine _gameEngine;
    public String gameStatus = "";

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
            gameStatus = "In Progress";
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

    [HttpGet("current-turn")]
    public IActionResult GetCurrentTurn(int gameId)
    {
        return Ok(_gameEngine.GetCurrentTurn().ToString());
    }

    [HttpPost("MovePiece")]
    public IActionResult MovePiece([FromBody] MoveRequest move)
    {
        var dbPieces = _gameService.LoadGame(move.GameId);
        var board = _gameEngine.BuildBoard(dbPieces);
        var gameStatus = _gameService.GetGameStatus(move.GameId);
        var attemptedTurn = move.UserId;

        var piece = board[move.From.X, move.From.Y];
        if (piece == null) {
            return Ok(new { validMove = false, message = "No piece at selected position" });
        }    

        string actualColor = piece.Color.ToString();
        int playerId = _gameService.GetPlayerIdByColor(move.GameId, actualColor);
        int currentTurn = _gameService.GetUserIdByGamePlayer(playerId);

        Debug.WriteLine($"Move requested by User {attemptedTurn}, piece color: {actualColor}, DB says user: {currentTurn}");


        if (currentTurn != attemptedTurn) {
            return Ok(new { validMove = false, message = $"It is not currently User {attemptedTurn}'s turn. The turn belongs to {currentTurn}" });
        }

        var result = _gameEngine.ValidateAndMovePiece(
            move.From, move.To, move.PieceType, piece.Color, gameStatus, board);

        if (result.Message == "Checkmate!" || result.Message == "Stalemate!")
        {
            Debug.WriteLine("So its either a checkmate or a stalemate");
            var status = result.Message == "Checkmate!" ? "Finished" : "Stalemate";
            int? winner = result.Message == "Checkmate!" ? currentTurn : (int?)null;
            _gameService.DeclareGameResult(move.GameId, winner, status);
        }

        if (result.IsValid)
        {
            var updated = _gameEngine.ConvertBoardToState(board);
            var nextTurn = _gameEngine.GetCurrentTurn().ToString();
            _gameService.SaveBoardToDatabase(move.GameId, updated);
            _gameService.SaveSnapshot(move.GameId, move.turnNumber, updated);
            return Ok(new { validMove = true, newPosition = result.NewPosition, message = result.Message });
        }

        return Ok(new { validMove = false, message = result.Message });
    }

    [HttpPost("fetchLegalMoves")]
    public IActionResult fetchLegalMoves([FromBody] LegalMoveRequest move)
    {
        var dbPieces = _gameService.LoadGame(move.GameId);
        var board = _gameEngine.BuildBoard(dbPieces);
        var legalMoves = _gameEngine.FindLegalMoves(move.From, board);
        var piece = board[move.From.X, move.From.Y];
        Debug.WriteLine($"Fetch from ({move.From.X},{move.From.Y}) -> {(board[move.From.X, move.From.Y] == null ? "NULL" : board[move.From.X, move.From.Y].PieceType.ToString())}");

        //if (piece == null)
        //{
        //    return BadRequest(new { message = "No piece at selected position" });
        //}
        return Ok(legalMoves);
    }

    [HttpPost("SwitchActiveUser")]
    public IActionResult SwitchActiveUser(SwitchUserRequest request)
    {
        var (whiteId, blackId) = _gameService.GetPlayerColors(request.gameId);

        if (request.CurrentUserId != whiteId && request.CurrentUserId != blackId) {
            return BadRequest(new { message = "CurrentUserId is not a player in this game." });
        }
        var next = (request.CurrentUserId == whiteId) ? blackId : whiteId;
        return Ok(new { nextUserId = next });
    }


}