using System.Diagnostics;
using ChessBackend;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChessController : ControllerBase

{
    private readonly GameService _gameService;

    public ChessController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("MoveKnight")]
    public IActionResult MoveKnight([FromBody] Coord newPosition)
    {
        Debug.WriteLine($"Request to move to: {newPosition.X}, {newPosition.Y}");
        var updatedPosition = _gameService.ValidateAndMoveKnight(newPosition);
        //var lastSelectedPiece = _gameService.LastSelectedPiece(lastSelectedPos);
        var knightPosition = _gameService.knightPosition;
        //if (lastSelectedPos == knightPosition) 
        {
            if (updatedPosition != null)
            {
                Debug.WriteLine("It actually works");
                return Ok(new
                {
                    validMove = true,
                    newPosition = updatedPosition
                });
            }
        } 
            Debug.WriteLine("It doesnt work");
        return Ok(new
        {
            validMove = false,
            newPosition = this.GetKnightPosition()
        });
    }

    [HttpGet("KnightPosition")]
    public IActionResult GetKnightPosition()
    {
        return Ok(_gameService.currentPosition);

    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello from ASP.NET Core!" });
        
    }
}
