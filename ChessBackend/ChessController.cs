using System.Diagnostics;
using ChessBackend;
using Microsoft.AspNetCore.Mvc;
using System.Timers;
using static ChessBackend.ChessPiece;
using Azure.Core.GeoJson;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]
public class ChessController : ControllerBase
{
    private static System.Timers.Timer aTimer;
    private readonly GameService _gameService;
    private readonly GameEngine _gameEngine;
    private readonly IConfiguration _configuration;



    public ChessController(GameService gameService, GameEngine gameEngine, IConfiguration configuration)
    {
        _gameService = gameService;
        _gameEngine = gameEngine;
        _configuration = configuration;
    }

    // Keep the original method for backward compatibility
    [HttpPost("MovePiece")]
    public IActionResult MovePiece(MoveRequest move)
    {
        Debug.WriteLine($"Request to move piece to: {move}");
        // Get current knight position (for backward compatibility)
        var currentPosition = _gameEngine.currentPosition;
        PieceType type = move.PieceType;

        // Validate and move the knight
        var result = _gameEngine.ValidateAndMovePiece(move.From, move.To, move.PieceType, _gameEngine.board);
        if (result.IsValid)
        {

            return Ok(new
            {
                validMove = true,
                newPosition = result.NewPosition
            });
        }
        else
        {
            return Ok(new
            {
                validMove = false,
                newPosition = currentPosition,
                message = result.Message
            });
        }
    }

    //// New method for all pieces
    //[HttpPost("MovePiece")]
    //public IActionResult MovePiece([FromBody] MoveRequest moveRequest)
    //{
    //    Debug.WriteLine($"22  Request to move from: {moveRequest.From.X}, {moveRequest.From.Y} to: {moveRequest.To.X}, {moveRequest.To.Y}");

    //    var result = _gameEngine.ValidateAndMovePiece(moveRequest.From, moveRequest.To);

    //    if (result.IsValid)
    //    {
    //        Debug.WriteLine("22 Move successful");
    //        return Ok(new
    //        {
    //            validMove = true,
    //            newPosition = moveRequest.To,
    //            pieceType = result.PieceType,
    //            pieceColor = result.PieceColor
    //        });
    //    }
    //    else
    //    {
    //        Debug.WriteLine("22 Invalid move");
    //        return Ok(new
    //        {
    //            validMove = false,
    //            message = result.Message,
    //            // Return current knight position for backward compatibility
    //            newPosition = _gameService.currentPosition
    //        });
    //    }
    //}

    [HttpPost("SelectPiece")]
    public IActionResult SelectPiece([FromBody] Coord position)
    {
        var piece = _gameEngine.GetPieceAt(position);

        if (piece != null)
        {
            return Ok(new
            {
                pieceFound = true,
                position = position,
                pieceType = (int)piece.PieceType,
                pieceColor = piece.Color.ToString()
            });
        }
        else
        {
            return Ok(new
            {
                pieceFound = false,
                position = position
            });
        }
    }


    [HttpPost("create-game")]
    public async Task<IActionResult> StartGame()
    {
        try
        {
            int newGameId = await _gameService.CreateGame();
            return Ok(new GameResponse { GameId = newGameId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create game", detail = ex.Message });
        }
    }

    private int GenerateNewGameId()
    {
        // Simulate a new Game ID generation (if using Identity column, it may be auto-generated)
        return new Random().Next(1000, 9999); // Example of random Game ID
    }

    // Keep this for backward compatibility
    [HttpGet("GetKnightPosition")]
    public IActionResult GetKnightPosition()
    {
        return Ok(_gameEngine.currentPosition);
    }

    [HttpGet("GetBoard")]
    public IActionResult GetBoard()
    {
        var boardState = _gameService.GetBoardState();
        return Ok(boardState);
    }
    [HttpGet("SelectedPiece")]
    public IActionResult SelectedPiece([FromQuery] int y, [FromQuery] int x)
    {
        var piece = _gameEngine.board[y, x];
        if (piece != null)
        {
            return Ok(new
            {
                y,
                x,
                pieceType = (int)piece.PieceType  // numeric value expected by frontend
            });
        }
        return NotFound("No piece at this location.");
    }


    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Chess API is running!" });
    }

    private static void SetTimer()
    {
        // Create a timer with a two second interval.
        aTimer = new System.Timers.Timer(2000);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                          e.SignalTime);
    }
}

// New class to handle move requests
public class MoveRequest
{
    public Coord From { get; set; }
    public Coord To { get; set; }
    public  PieceType PieceType { get; set; }
}

public class GameResponse
{
    public int GameId { get; set; }
}