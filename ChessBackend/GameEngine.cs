using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Formatters;
using static ChessBackend.ChessPiece;

namespace ChessBackend
{
    public class GameEngine
    {
        private PieceColor currentTurn;
        public Coord? LastDoubleStepPawnPosition = null;

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

            currentTurn = PieceColor.White;

            return pieces;
        }

        //Validate and apply a move on a temporary board
        public MoveResult ValidateAndMovePiece(Coord from, Coord to, PieceType type, PieceColor color, String gameStatus, ChessPiece[,] board)
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

            if (gameStatus == "Finished")
            {
                return new MoveResult { IsValid = false, Message = "Game complete" };
            }


            bool moved = false;

            switch ((int)type)
            {
                case 0:
                    if (IsEnPassant(from, to, board))
                    {
                        int captureX = from.X;
                        int captureY = to.Y;
                        ChessPiece capturedPawn = board[captureX, captureY];
                        if (capturedPawn != null)
                        {
                            capturedPawn.isCaptured = 1;
                            board[captureX, captureY] = null;
                            moved = true;
                        }

                        moved = true;
                    } else
                    if (CanPromote(from, to, board))
                    {
                        piece.PieceType = PieceType.Queen;
                        moved = true;
                    }
                    else if (piece.CanMovePawn(to, board)) moved = true; break;
                case 1: if (piece.CanMoveKnight(to, board)) moved = true; break;
                case 2: if (piece.CanMoveBishop(to, board)) moved = true; break;
                case 3: if (piece.CanMoveRook(to, board)) moved = true; break;
                case 4: if (piece.CanMoveQueen(to, board)) moved = true; break;
                case 5:
                    if (CanCastle(from, to, board))
                    {
                        // 1. Move the king
                        board[to.X, to.Y] = piece;
                        board[from.X, from.Y] = null;
                        piece.SetPosition(to);
                        piece.hasMoved = true;

                        // 2. Move the rook
                        bool isKingside = to.Y > from.Y;
                        int rookStartY = isKingside ? 7 : 0;
                        int rookEndY = isKingside ? 5 : 3;
                        ChessPiece rook = board[from.X, rookStartY];
                        board[from.X, rookEndY] = rook;
                        board[from.X, rookStartY] = null;
                        rook.SetPosition(new Coord(from.X, rookEndY));
                        rook.hasMoved = true;

                        moved = true;
                    }
                    else if (piece.CanMoveKing(to, board))
                    {
                        moved = true;
                    }
                    break;
                default: return new MoveResult { IsValid = false, Message = "Unknown piece type" };
            }

            if (!moved)
                return new MoveResult { IsValid = false, Message = "Invalid move for this piece" };
            piece.hasMoved = true;

            var target = board[to.X, to.Y];
            if (target != null && target.Color != piece.Color)
            {
                if (target.PieceType == PieceType.King)
                    return new MoveResult { IsValid = false, Message = "You cannot capture the king" };

                target.isCaptured = 1;
            }

            board[to.X, to.Y] = piece;
            board[from.X, from.Y] = null;
            piece.SetPosition(to);

            var opponentColor = (piece.Color == PieceColor.White) ? PieceColor.Black : PieceColor.White;



            if (IsInCheck(opponentColor, board) && !HasLegalMoves(opponentColor, board))
            {
                Debug.WriteLine($"IsInCheck for {opponentColor}: {IsInCheck(opponentColor, board)}");
                Debug.WriteLine($"HasLegalMoves for {opponentColor}: {HasLegalMoves(opponentColor, board)}");
                return new MoveResult
                {
                    IsValid = true,
                    Message = "Checkmate!",
                    NewPosition = to,
                    PieceColor = piece.Color.ToString(),
                    PieceType = (int)piece.PieceType,
                    WinnerUserId = 1
                };
            }
            else if (!IsInCheck(opponentColor, board) && !HasLegalMoves(opponentColor, board))
            {
                Debug.WriteLine("Frick its not working");
                return new MoveResult
                {
                    IsValid = true,
                    Message = "Stalemate!",
                    NewPosition = to,
                    PieceColor = piece.Color.ToString(),
                    PieceType = (int)piece.PieceType
                };
            }

            if (piece.PieceType == PieceType.Pawn && Math.Abs(to.X - from.X) == 2)
            {
                LastDoubleStepPawnPosition = to;
            }
            else
            {
                LastDoubleStepPawnPosition = null;
            }

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

        public bool IsEnPassant(Coord from, Coord to, ChessPiece[,] board)
        {
            ChessPiece piece = board[from.X, from.Y];
            if (piece.PieceType != PieceType.Pawn || board[to.X, to.Y] != null)
                return false;

            if (LastDoubleStepPawnPosition == null)
                return false;

            var last = LastDoubleStepPawnPosition;
            if (Math.Abs(to.Y - from.Y) != 1)
                return false;

            if (piece.Color == PieceColor.White)
            {
                if (to.X != from.X - 1) return false;
                return last.X == from.X && last.Y == to.Y;
            }
            if (piece.Color == PieceColor.Black)
            {
                if (to.X != from.X + 1) return false;
                return last.X == from.X && last.Y == to.Y;
            }

            return false;
        }

        private bool CanCastle(Coord from, Coord to, ChessPiece[,] board)
        {
            if (Math.Abs(to.Y - from.Y) != 2 || from.X != to.X)
                return false;
            ChessPiece king = board[from.X, from.Y];
            if (king == null || king.PieceType != PieceType.King || king.hasMoved)
                return false;
            bool isKingside = to.Y > from.Y;
            int rookCol = isKingside ? 7 : 0;
            ChessPiece rook = board[from.X, rookCol];
            if (rook == null || rook.PieceType != PieceType.Rook || rook.hasMoved)
                return false;

            return true;
        }

        private bool CanPromote(Coord from, Coord to, ChessPiece[,] board)
        {
            ChessPiece piece = board[from.X, from.Y];
            if (piece == null || piece.PieceType != PieceType.Pawn)
                return false;

            return (piece.Color == PieceColor.White && to.X == 0) ||
                   (piece.Color == PieceColor.Black && to.X == 7);
        }

        // --- Helpers ---
        private static bool InBounds(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;

        private static int PawnDir(PieceColor color) => color == PieceColor.White ? -1 : 1; // White goes "up" (X-1)
        private static int PawnStartRank(PieceColor color) => color == PieceColor.White ? 6 : 1;

        private static bool PawnAttacksSquare(ChessPiece pawn, Coord from, Coord target)
        {
            int dir = PawnDir(pawn.Color);
            return target.X == from.X + dir && (target.Y == from.Y - 1 || target.Y == from.Y + 1);
        }

        private static bool SquareEmpty(ChessPiece[,] board, int x, int y) => board[x, y] == null;
        private static bool SquareHasEnemy(ChessPiece[,] board, int x, int y, PieceColor me)
            => InBounds(x, y) && board[x, y] != null && board[x, y].Color != me;


        public bool IsInCheck(PieceColor color, ChessPiece[,] board)
        {
            Coord kingPos = FindKing(color, board);

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    var piece = board[x, y];
                    if (piece == null || piece.Color == color) continue;

                    var from = new Coord(x, y);

                    switch (piece.PieceType)
                    {
                        case PieceType.Pawn:
                            if (PawnAttacksSquare(piece, from, kingPos)) return true;
                            break;

                        case PieceType.Knight:
                            if (piece.CanMoveKnight(kingPos, board)) return true;
                            break;

                        case PieceType.Bishop:
                            if (piece.CanMoveBishop(kingPos, board)) return true;
                            break;

                        case PieceType.Rook:
                            if (piece.CanMoveRook(kingPos, board)) return true;
                            break;

                        case PieceType.Queen:
                            if (piece.CanMoveQueen(kingPos, board)) return true;
                            break;

                        case PieceType.King:
                            if (Math.Abs(kingPos.X - from.X) <= 1 && Math.Abs(kingPos.Y - from.Y) <= 1) return true;
                            break;
                    }
                }

            return false;
        }

        public bool HasLegalMoves(PieceColor color, ChessPiece[,] board)
        {
            for (int fromX = 0; fromX < 8; fromX++)
                for (int fromY = 0; fromY < 8; fromY++)
                {
                    var piece = board[fromX, fromY];
                    if (piece == null || piece.Color != color) continue;

                    var from = new Coord(fromX, fromY);

                    // Generate candidates
                    IEnumerable<Coord> Candidates()
                    {
                        switch (piece.PieceType)
                        {
                            case PieceType.Pawn:
                                {
                                    int dir = PawnDir(color);
                                    int start = PawnStartRank(color);

                                    // forward 1
                                    int f1x = from.X + dir, f1y = from.Y;
                                    if (InBounds(f1x, f1y) && SquareEmpty(board, f1x, f1y))
                                        yield return new Coord(f1x, f1y);

                                    // forward 2 from start, if both empty
                                    int f2x = from.X + 2 * dir;
                                    if (from.X == start && InBounds(f2x, f1y) && SquareEmpty(board, f2x, f1y) && SquareEmpty(board, f1x, f1y))
                                        yield return new Coord(f2x, f1y);

                                    // diagonal captures
                                    int cx = from.X + dir;
                                    int cyL = from.Y - 1, cyR = from.Y + 1;
                                    if (InBounds(cx, cyL) && SquareHasEnemy(board, cx, cyL, color))
                                        yield return new Coord(cx, cyL);
                                    if (InBounds(cx, cyR) && SquareHasEnemy(board, cx, cyR, color))
                                        yield return new Coord(cx, cyR);

                                    // optional: en passant (if you already track it)
                                    if (IsEnPassant(from, new Coord(cx, cyL), board)) yield return new Coord(cx, cyL);
                                    if (IsEnPassant(from, new Coord(cx, cyR), board)) yield return new Coord(cx, cyR);
                                    break;
                                }

                            case PieceType.Knight:
                            case PieceType.Bishop:
                            case PieceType.Rook:
                            case PieceType.Queen:
                            case PieceType.King:
                                {
                                    for (int toX = 0; toX < 8; toX++)
                                        for (int toY = 0; toY < 8; toY++)
                                        {
                                            if (toX == from.X && toY == from.Y) continue;
                                            var to = new Coord(toX, toY);

                                            bool ok = piece.PieceType switch
                                            {
                                                PieceType.Knight => piece.CanMoveKnight(to, board),
                                                PieceType.Bishop => piece.CanMoveBishop(to, board),
                                                PieceType.Rook => piece.CanMoveRook(to, board),
                                                PieceType.Queen => piece.CanMoveQueen(to, board),
                                                PieceType.King => piece.CanMoveKing(to, board),
                                                _ => false
                                            };

                                            if (ok) yield return to;
                                        }
                                    break;
                                }
                        }
                    }

                    foreach (var to in Candidates())
                    {
                        // simulate
                        var originalTarget = board[to.X, to.Y];
                        var originalPos = piece.CurrentPosition;

                        board[to.X, to.Y] = piece;
                        board[from.X, from.Y] = null;
                        piece.SetPosition(to);

                        bool inCheck = IsInCheck(color, board);

                        // undo
                        board[from.X, from.Y] = piece;
                        board[to.X, to.Y] = originalTarget;
                        piece.SetPosition(originalPos);

                        if (!inCheck) return true; // found at least one legal move
                    }
                }

            return false;
        }



        private Coord FindKing(PieceColor color, ChessPiece[,] board)
        {
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    var p = board[x, y];
                    if (p != null && p.PieceType == PieceType.King && p.Color == color)
                        return new Coord(x, y);
                }
            throw new InvalidOperationException($"King of color {color} not found.");
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
            public int WinnerUserId { get; set; }
        }
    }
}
