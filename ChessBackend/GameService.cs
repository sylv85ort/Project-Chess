using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ChessBackend
{
    public class GameService
    {
        private readonly IConfiguration _configuration;
        private readonly GameEngine _gameEngine;
        public ChessPiece SelectedPiece { get; private set; }

        // Keep this for backward compatibility with frontend

        public GameService(GameEngine gameEngine, IConfiguration configuration)
        {
            _gameEngine = gameEngine;
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        //private void InitializeBoard()
        //{
        //    var knight = new Knight(PieceColor.White);
        //    board[currentPosition.X, currentPosition.Y] = knight;
        //    knight.SetPosition(currentPosition);

        //    // Add more pieces as needed
        //    // Example: Add a black knight
        //    Coord blackKnightPos = new Coord(7, 0);
        //    var blackKnight = new Knight(PieceColor.Black); 
        //    board[blackKnightPos.X, blackKnightPos.Y] = blackKnight;
        //    blackKnight.SetPosition(blackKnightPos);
        //}

        //public Coord ValidateAndMoveKnight(Coord from, Coord to, ChessPiece[,] externalBoard = null)
        //{
        //    ChessPiece[,] boardToUse = externalBoard ?? this.board;

        //    if (!IsValidCoordinate(from) || boardToUse[from.X, from.Y] == null)
        //    {
        //        return null;
        //    }

        //    ChessPiece piece = boardToUse[from.X, from.Y];

        //    if (!(piece is Knight))
        //    {
        //        return null;
        //    }


        //    if (piece.CanMovePiece(to, boardToUse))
        //    {

        //        boardToUse[to.X, to.Y] = piece;
        //        boardToUse[from.X, from.Y] = null;
        //        piece.SetPosition(to);

        //        if (piece.Color == PieceColor.White && piece is Knight)
        //        {
        //            currentPosition = to;
        //        }

        //        return to;
        //    }

        //    return null;
        //}


        //Currently not using.
        //    public MoveResult ValidateAndMovePiece(Coord from, Coord to)
        //{
        //    // Check if coordinates are valid
        //    if (!IsValidCoordinate(from) || !IsValidCoordinate(to))
        //    {
        //        return new MoveResult
        //        {
        //            IsValid = false,
        //            Message = "Invalid coordinates"
        //        };
        //    }

        //    // Check if there is a piece at the from position
        //    ChessPiece piece = board[from.X, from.Y];
        //    if (piece == null)
        //    {
        //        return new MoveResult
        //        {
        //            IsValid = false,
        //            Message = "No piece at selected position"
        //        };
        //    }

        //    // Check if the piece can move to the target position
        //    if (piece.CanMovePiece(to, board))
        //    {
        //        // Perform the move
        //        board[to.X, to.Y] = piece;
        //        board[from.X, from.Y] = null;
        //        piece.SetPosition(to);

        //        // Update currentPosition for backward compatibility
        //        if (piece.Color == PieceColor.White && piece is Knight)
        //        {
        //            currentPosition = to;
        //        }

        //        return new MoveResult
        //        {
        //            IsValid = true,
        //            PieceType = piece.GetType().Name,
        //            PieceColor = piece.Color.ToString()
        //        };
        //    }
        //    else
        //    {
        //        return new MoveResult
        //        {
        //            IsValid = false,
        //            Message = "Invalid move for this piece"
        //        };
        //    }
        //}

        public async Task<int> CreateGame()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    // Insert new game
                    string insertQuery = "INSERT INTO Games (gameStatus) VALUES ('waiting');";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        await insertCmd.ExecuteNonQueryAsync();
                    }

                    // Retrieve GameID
                    string idQuery = "SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    using (SqlCommand idCmd = new SqlCommand(idQuery, conn))
                    {
                        var result = await idCmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    throw; // rethrow to be handled by controller if needed
                }
            }
        }



        // thiss basically reflect the current state of the board including position and type of each piece
        public object GetBoardState()
        {
            var boardState = new List<object>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    ChessPiece piece = _gameEngine.board[x, y];
                    if (piece != null)
                    {
                        boardState.Add(new
                        {
                            position = new { x = x, y = y },
                            pieceType = (int)piece.PieceType,
                            pieceColor = piece.Color.ToString()
                        });
                    }
                }
            }

            return boardState;
        }


    }
}