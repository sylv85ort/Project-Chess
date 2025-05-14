using System.Data.SqlTypes;
using System.Diagnostics;
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

        public ChessPiece(Coord position, PieceColor color, PieceType pieceType, int pieceID)
        {
            CurrentPosition = position;
            Color = color;
            this.pieceID = pieceID;
            this.PieceType = pieceType;
            gamePlayerID = -1;
            isCaptured = 0;  // or whatever default makes sense
        }

        public void CheckPieceType(Coord to, PieceType type, ChessPiece[,] board)
        {
            if ((int)type == 0)
            {
                //CanMovPawn(to, board);
                CanMovePawn(to, board);
                return;
            }
            else if ((int)type == 1)
            {
                CanMoveKnight(to, board);
                return;
            }
        }

        //SqlConnection connection = new SqlConnection("DefaultConnection");
        //string sqlText = "use ProjectChessDB" +
        //    "select gameID, gamePlayerID, pieceID, from GamePiece";
        //SqlCommand command = new SqlCommand(sqlText, connection);
        //SqlDataReader reader = command.ExecuteReader();
        //    while (reader.Read()) {
        //        gamePlayerID.Equals(reader["gamePlayerID"]);
        //    }

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
            //X IS Y AND Y IS X NO TIME TO EXPLAIN
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            //bool is2Move = (Math.Abs(dx) == 2);

            //if (!is2Move) return false;
            if (Color == PieceColor.White && dx >= 0)
                return false; // White can't move backward or stay
            if (Color == PieceColor.Black && dx <= 0)
                return false; // Black can't move backward or stay
            if (Color == PieceColor.White && CurrentPosition.X == 6)
            {
                return (Math.Abs(dx) == 2) || (Math.Abs(dx) == 1);
            }
            else if (Color == PieceColor.White)
            {
                return (Math.Abs(dx) == 1);
            }
            if (Color == PieceColor.Black && CurrentPosition.X == 1)
            {
                return (Math.Abs(dx) == 2);
            }
            else if (Color == PieceColor.Black)
            {
                return (Math.Abs(dx) == 1);
            }

            //PROMOTION IS EASYYYYY
            //CHECK IF WHITE THEN CHECK on 6 AND VICE VERSA
            //IF TRUE CHANGE TYPE TO ANOTHER TYPE NA MEAN
            //SET IS PROMOTED TO YES 
            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;  // Can't capture own piece

            return true;
        }


        public bool CanMoveKnight(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            // L-shape movement: 2 squares in one direction and 1 square perpendicular
            bool isLShape = (Math.Abs(dx) == 2 && Math.Abs(dy) == 1) ||
                           (Math.Abs(dx) == 1 && Math.Abs(dy) == 2);

            if (!isLShape) return false;

            // Check if target square is empty or has an enemy piece
            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;  // Can't capture own piece

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

            //// L-shape movement: 2 squares in one direction and 1 square perpendicular
            //bool isLShape = (Math.Abs(dx) == 1 && Math.Abs(dy) == 1) ||
            //               (Math.Abs(dx) == 1 && Math.Abs(dy) == 1);
            //if (!isLShape) return false;

            while (x != to.X && y != to.Y)
            {
                if (board[x, y] != null)
                {
                    return false;
                    //break;
                }


                x += stepX;
                y += stepY;
            }
            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;  // Can't capture own piece

            return true;

        }

        public bool CanMoveRook(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            if (Math.Abs(dx) != 0 && Math.Abs(dy) != 0)
                return false;


            int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
            int stepY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

            int x = CurrentPosition.X + stepX;
            int y = CurrentPosition.Y + stepY;


             while (x != to.X )
            {
                if (x < 0 || x >= 8)
                    return false; // Don't go off the board

                if (board[x, y] != null)
                {
                    return false;
                    break;
                }


                x += stepX;
                y += stepY;
            }

                while (y != to.Y)
                {
                    if (y < 0 || y >= 8)
                        return false; // Don't go off the board

                    if (board[x, y] != null)
                    {
                        return false;
                    }


                    x += stepX;
                    y += stepY;
                }

            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;  // Can't capture own piece

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


            if (dx != 0)
            {
                while (x != to.X)
                {
                    if (x < 0 || x >= 8)
                    {
                        return false;
                    }
                    if (board[x, y] != null)
                    {
                        Debug.WriteLine("WHat rgrthe fuck");
                        return false;
                    }
                    x += stepX;
                }
            } else if (y != to.Y)
            {   while (y != to.Y)
                {
                    if (y < 0 || y >= 8)
                    {
                        return false;
                    }
                    if (board[x, y] != null)
                    {
                        Debug.WriteLine("WHat rgthe fuck");
                        return false;
                    }
                    y += stepY;
                }
            } else
                while (x != to.X && y != to.Y)
                {
                    if (x < 0 || x >= 8 || y < 0 || y >= 8)
                    {
                        return false; // Don't go off the board
                    }



                    if (board[x, y] != null)
                    {
                        Debug.WriteLine("WHat the fuck");
                        return false;
                    }

                    x += stepX;
                    y += stepY;
                }
            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
                return false;  // Can't capture own piece

            return true;

        }

        public bool CanMoveKing(Coord to, ChessPiece[,] board)
        {
            int dx = to.X - CurrentPosition.X;
            int dy = to.Y - CurrentPosition.Y;

            // Check if the move is only 1 square in any direction (including diagonals)
            if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
            {
                return false; // King cannot move more than one square
            }

            // Check if the destination is within the board
            if (to.X < 0 || to.X >= 8 || to.Y < 0 || to.Y >= 8)
            {
                return false; // Out of bounds
            }

            // Check if the target square is occupied by a piece of the same color
            ChessPiece target = board[to.X, to.Y];
            if (target != null && target.Color == this.Color)
            {
                return false; // Can't capture your own piece
            }

            // Otherwise, the move is valid
            return true;
        }

    }
}