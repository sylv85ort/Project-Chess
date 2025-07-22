using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ChessBackend.ChessPiece;

namespace ChessBackend
{
    public class GameEngine
    {
        private PieceColor currentTurn;

        public GameEngine()
        {
            currentTurn = PieceColor.White; // Game starts with White's turn
        }

        //Build a temporary 8x8 board from DB data
        public ChessPiece[,] BuildBoard(List<object> dbPieces)
        {
            var board = new ChessPiece[8, 8];

            foreach (dynamic piece in dbPieces)
            {
                int x = piece.position.x;
                int y = piece.position.y;
                int pieceType = piece.pieceType;
                string colorStr = piece.pieceColor;
                PieceColor color = (PieceColor)Enum.Parse(typeof(PieceColor), colorStr);

                board[x, y] = new ChessPiece(new Coord(x, y), color, (PieceType)pieceType, 0);
            }

            return board;
        }

        //Convert an in-memory board back to a savable state for DB
        public List<object> ConvertBoardToState(ChessPiece[,] board)
        {
            var boardState = new List<object>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var piece = board[x, y];
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

        //Return the initial starting position of the board
        public List<object> GetInitialBoardState()
        {
            var pieces = new List<object>();

            // White Pawns
            for (int i = 0; i < 8; i++)
                pieces.Add(new { position = new { x = 6, y = i }, pieceType = 0, pieceColor = "White" });

            // Black Pawns
            for (int i = 0; i < 8; i++)
                pieces.Add(new { position = new { x = 1, y = i }, pieceType = 0, pieceColor = "Black" });

            // White Back Rank
            pieces.Add(new { position = new { x = 7, y = 0 }, pieceType = 3, pieceColor = "White" }); // Rook
            pieces.Add(new { position = new { x = 7, y = 7 }, pieceType = 3, pieceColor = "White" }); // Rook
            pieces.Add(new { position = new { x = 7, y = 1 }, pieceType = 1, pieceColor = "White" }); // Knight
            pieces.Add(new { position = new { x = 7, y = 6 }, pieceType = 1, pieceColor = "White" }); // Knight
            pieces.Add(new { position = new { x = 7, y = 2 }, pieceType = 2, pieceColor = "White" }); // Bishop
            pieces.Add(new { position = new { x = 7, y = 5 }, pieceType = 2, pieceColor = "White" }); // Bishop
            pieces.Add(new { position = new { x = 7, y = 3 }, pieceType = 4, pieceColor = "White" }); // Queen
            pieces.Add(new { position = new { x = 7, y = 4 }, pieceType = 5, pieceColor = "White" }); // King

            // Black Back Rank
            pieces.Add(new { position = new { x = 0, y = 0 }, pieceType = 3, pieceColor = "Black" });
            pieces.Add(new { position = new { x = 0, y = 7 }, pieceType = 3, pieceColor = "Black" });
            pieces.Add(new { position = new { x = 0, y = 1 }, pieceType = 1, pieceColor = "Black" });
            pieces.Add(new { position = new { x = 0, y = 6 }, pieceType = 1, pieceColor = "Black" });
            pieces.Add(new { position = new { x = 0, y = 2 }, pieceType = 2, pieceColor = "Black" });
            pieces.Add(new { position = new { x = 0, y = 5 }, pieceType = 2, pieceColor = "Black" });
            pieces.Add(new { position = new { x = 0, y = 3 }, pieceType = 4, pieceColor = "Black" });
            pieces.Add(new { position = new { x = 0, y = 4 }, pieceType = 5, pieceColor = "Black" });

            return pieces;
        }

        //Validate and apply a move on a temporary board
        public MoveResult ValidateAndMovePiece(Coord from, Coord to, PieceType type, PieceColor color, ChessPiece[,] board)
        {
            if (!IsValidCoordinate(from) || !IsValidCoordinate(to))
                return new MoveResult { IsValid = false, Message = "Invalid coordinates" };

            var piece = board[from.X, from.Y];
            if (piece == null)
                return new MoveResult { IsValid = false, Message = "No piece at selected position" };

            if (piece.Color != currentTurn)
                return new MoveResult { IsValid = false, Message = $"It's {currentTurn}'s turn" };

            if (type != piece.PieceType)
                return new MoveResult { IsValid = false, Message = "Piece type mismatch" };

            bool moved = false;

            switch ((int)type)
            {
                case 0: if (piece.CanMovePawn(to, board)) moved = true; break;
                case 1: if (piece.CanMoveKnight(to, board)) moved = true; break;
                case 2: if (piece.CanMoveBishop(to, board)) moved = true; break;
                case 3: if (piece.CanMoveRook(to, board)) moved = true; break;
                case 4: if (piece.CanMoveQueen(to, board)) moved = true; break;
                case 5: if (piece.CanMoveKing(to, board)) moved = true; break;
                default: return new MoveResult { IsValid = false, Message = "Unknown piece type" };
            }

            if (!moved)
                return new MoveResult { IsValid = false, Message = "Invalid move for this piece" };

            //Apply move
            board[to.X, to.Y] = piece;
            board[from.X, from.Y] = null;
            piece.SetPosition(to);

            currentTurn = (currentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;

            return new MoveResult
            {
                IsValid = true,
                Message = "Move successful",
                NewPosition = to,
                PieceColor = piece.Color.ToString(),
                PieceType = (int)piece.PieceType
            };
        }

        private bool IsValidCoordinate(Coord coord) =>
            coord != null && coord.X >= 0 && coord.X < 8 && coord.Y >= 0 && coord.Y < 8;

        public class MoveResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
            public Coord NewPosition { get; set; }
            public int PieceType { get; set; }
            public string PieceColor { get; set; }
        }
    }
}
