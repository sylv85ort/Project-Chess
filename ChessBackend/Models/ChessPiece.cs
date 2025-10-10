using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO.Pipelines;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChessBackend
{
    public enum PieceType
    {
        None = -1,
        Pawn = 0,
        Knight = 1,
        Bishop = 2,
        Rook = 3,
        Queen = 4,
        King = 5
    }

    public class ChessPiece
    {
        public Coord CurrentPosition { get; set; }
        public PieceColor Color { get; set; }
        public int pieceID { get; set; }
        public PieceType PieceType { get; set; }
        public int gamePlayerID { get; set; }
        public int isCaptured { get; set; }

        public bool hasMoved { get; set; } = false;
        public ChessPiece(Coord position, PieceColor color, PieceType pieceType, int pieceID)
        {
            CurrentPosition = position;
            Color = color;
            this.pieceID = pieceID;
            this.PieceType = pieceType;
            gamePlayerID = -1;
            isCaptured = 0;
        }

        public void CheckPieceType(Coord to, PieceType type, ChessPiece[,] board)
        {
            if ((int)type == 0)
            {
                CanMovePawn(to, board);
                return;
            }
            else if ((int)type == 1)
            {
                CanMoveKnight(to, board);
                return;
            }
        }

        public void SetPosition(Coord position)
        {
            CurrentPosition = position;
        }

        public PieceColor GetColor()
        {
            return Color;
        }

        public bool CanMovePawn(Coord to, ChessPiece[,] board)
        {
            var from = CurrentPosition;
            int dx = to.X - from.X;   
            int dy = to.Y - from.Y;

            if (to.X < 0 || to.X >= 8 || to.Y < 0 || to.Y >= 8) return false;
            if (dx == 0 && dy == 0) return false;

            var target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color) return false;

            int dir = (Color == PieceColor.White) ? -1 : +1;  
            int start = (Color == PieceColor.White) ? 6 : 1;

            if (dy == 0)
            {
                if (target != null) return false;

                if (dx == dir) return true;

                if (from.X == start && dx == 2 * dir)
                {
                    int midX = from.X + dir;
                    if (board[midX, from.Y] == null) return true;
                }
                return false;
            }

            if (Math.Abs(dy) == 1 && dx == dir)
            {
                return target != null && target.Color != this.Color;
            }
            return false;
        }


        public bool CanMoveKnight(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            bool isLShape = (Math.Abs(dx) == 2 && Math.Abs(dy) == 1) ||
                           (Math.Abs(dx) == 1 && Math.Abs(dy) == 2);

            if (!isLShape) return false;

            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;

            return true;
        }

        public bool CanMoveBishop(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            if (Math.Abs(dx) != Math.Abs(dy))
                return false;

            int stepX = dx > 0 ? 1 : -1;
            int stepY = dy > 0 ? 1 : -1;

            int x = CurrentPosition.X + stepX;
            int y = CurrentPosition.Y + stepY;

            while (x != to.X && y != to.Y)
            {
                if (x < 0 || x > 7 || y < 0 || y > 7) return false;
                if (board[x, y] != null)
                {
                    return false;
                }


                x += stepX;
                y += stepY;
            }
            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
            {
                return false;
            }

            return true;

        }

        public bool CanMoveRook(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            if (Math.Abs(dx) != 0 && Math.Abs(dy) != 0)
            {
                return false;
            }

            int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
            int stepY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

            int x = CurrentPosition.X + stepX;
            int y = CurrentPosition.Y + stepY;


             while (x != to.X )
            {
                if (x < 0 || x >= 8)
                    return false;

                if (board[x, y] != null)
                {
                    return false;
                }


                x += stepX;
                y += stepY;
            }

                while (y != to.Y)
                {
                    if (y < 0 || y >= 8)
                        return false;

                    if (board[x, y] != null)
                    {
                        return false;
                    }


                    x += stepX;
                    y += stepY;
                }

            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;

            return true;

        }

        public bool CanMoveQueen(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            if (!(Math.Abs(dx) == Math.Abs(dy) || dx == 0 || dy == 0))
                return false;

            int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
            int stepY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

            int x = CurrentPosition.X + stepX;
            int y = CurrentPosition.Y + stepY;

            while (x != to.X || y != to.Y)
            {
                if (x < 0 || x >= 8 || y < 0 || y >= 8)
                    return false;

                if (board[x, y] != null)
                {
                    Debug.WriteLine("Path is blocked");
                    return false;
                }

                x += stepX;
                y += stepY;
            }

            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;

            return true;
        }

        public bool CanMoveKing(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
            {
                return false; 
            }

            if (to.X < 0 || to.X >= 8 || to.Y < 0 || to.Y >= 8)
            {
                return false;
            }

            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
            {
                return false;
            }

            return true;
        }

    }
}