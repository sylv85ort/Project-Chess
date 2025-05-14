using System.Diagnostics;
using static ChessBackend.ChessPiece;

namespace ChessBackend
{
    public class GameEngine
    {
        public ChessPiece[,] board;
        private Coord knightPos = new Coord(1,5);
        private Coord pawnPos = new Coord(2, 6);


        public ChessPiece SelectedPiece { get; private set; }

        // Keep this for backward compatibility with frontend
        public Coord currentPosition { get; private set; }

        public GameEngine()
        {
            board = new ChessPiece[8, 8];
            currentPosition = knightPos;
            InitializeBoard();
        }


        private void InitializeBoard()
        {
            //GetBoard();
            //var knight = new ChessPiece(knightPos, PieceColor.White, ChessPiece.PieceType.Knight, 1);
            //var pawn = new ChessPiece(pawnPos, PieceColor.White, ChessPiece.PieceType.Pawn, 2);

            //var pieces = new List<(Coord pos, PieceColor color, PieceType type, int id)>
            //{
            //    (new Coord(1, 5), PieceColor.White, PieceType.Knight, 1),
            //    (new Coord(4, 6), PieceColor.White, PieceType.Pawn, 2),
            //    (new Coord(0, 0), PieceColor.White, PieceType.Bishop, 3),
            //    (new Coord(0, 1), PieceColor.White, PieceType.Rook, 4),
            //    (new Coord(0, 4), PieceColor.White, PieceType.Queen, 5),
            //    (new Coord(3, 7), PieceColor.White, PieceType.King, 6),
            //    (new Coord(4, 7), PieceColor.White, PieceType.Pawn, 7),
            //};

            var pieces = new List<(Coord pos, PieceColor color, PieceType type, int id)>
            {
                (new Coord(6, 0), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(6, 1), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(6, 2), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(6, 3), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(6, 4), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(6, 5), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(6, 6), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(6, 7), PieceColor.White, PieceType.Pawn, 2),
                (new Coord(7, 0), PieceColor.White, PieceType.Rook, 9),
                (new Coord(7, 1), PieceColor.White, PieceType.Knight, 10),
                (new Coord(7, 2), PieceColor.White, PieceType.Bishop, 3),
                (new Coord(7, 3), PieceColor.White, PieceType.King, 6),
                (new Coord(7, 4), PieceColor.White, PieceType.Queen, 6),
                (new Coord(7, 5), PieceColor.White, PieceType.Bishop, 3),
                (new Coord(7, 6), PieceColor.White, PieceType.Knight, 10),
                (new Coord(7, 7), PieceColor.White, PieceType.Rook, 16),
                                (new Coord(1, 0), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(1, 1), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(1, 2), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(1, 3), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(1, 4), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(1, 5), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(1, 6), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(1, 7), PieceColor.Black, PieceType.Pawn, 2),
                (new Coord(0, 0), PieceColor.Black, PieceType.Rook, 9),
                (new Coord(0, 1), PieceColor.Black, PieceType.Knight, 10),
                (new Coord(0, 2), PieceColor.Black, PieceType.Bishop, 3),
                (new Coord(0, 3), PieceColor.Black, PieceType.King, 6),
                (new Coord(0, 4), PieceColor.Black, PieceType.Queen, 6),
                (new Coord(0, 5), PieceColor.Black, PieceType.Bishop, 3),
                (new Coord(0, 6), PieceColor.Black, PieceType.Knight, 10),
                (new Coord(0, 7), PieceColor.Black, PieceType.Rook, 16),
            };

            //var BlackPieces = new List<(Coord pos, PieceColor color, PieceType type, int id)>
            //{
            //    (new Coord(1, 0), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(1, 1), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(1, 2), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(1, 3), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(1, 4), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(1, 5), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(1, 6), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(1, 7), PieceColor.Black, PieceType.Pawn, 2),
            //    (new Coord(0, 0), PieceColor.Black, PieceType.Rook, 9),
            //    (new Coord(0, 1), PieceColor.Black, PieceType.Knight, 10),
            //    (new Coord(0, 2), PieceColor.Black, PieceType.Bishop, 3),
            //    (new Coord(0, 3), PieceColor.Black, PieceType.King, 6),
            //    (new Coord(0, 4), PieceColor.Black, PieceType.Queen, 6),
            //    (new Coord(0, 5), PieceColor.Black, PieceType.Bishop, 3),
            //    (new Coord(0, 6), PieceColor.Black, PieceType.Knight, 10),
            //    (new Coord(0, 7), PieceColor.Black, PieceType.Rook, 16),
            //};



            foreach (var (pos, color, type, id) in pieces)
            {
                var piece = new ChessPiece(pos, color, type, id);
                board[pos.X, pos.Y] = piece;

                // Debugging statement to ensure the piece type is correctly set
                Debug.WriteLine($"Piece at ({pos.X}, {pos.Y}): {piece.PieceType}");
            }


            //var pawn = new Pawn(PieceColor.White);
            //knight.SetPosition(currentPosition);
            //board[knightPos.X, knightPos.Y] = knight;


            // Add more pieces as needed
            // Example: Add a black knightv
            //Coord blackKnightPos = new Coord(7, 0);
            //var blackKnight = new Knight(PieceColor.Black);
            //board[blackKnightPos.X, blackKnightPos.Y] = blackKnight;
            //blackKnight.SetPosition(blackKnightPos);
        }

        private bool IsValidCoordinate(Coord coord)
        {
            return coord != null && coord.X >= 0 && coord.X < 8 && coord.Y >= 0 && coord.Y < 8;
        }


        public ChessPiece GetPieceAt(Coord position)
        {
            if (!IsValidCoordinate(position))
                return null;

            return board[position.X, position.Y];
        }


        //public Coord ValidateAndMoveKnight(Coord from, Coord to, ChessPiece.PieceType pieceType, ChessPiece[,] board)
        //{

        //    if (!IsValidCoordinate(from) || board[from.X, from.Y] == null)
        //    {
        //        return null;
        //    }

        //    ChessPiece piece = board[from.X, from.Y];

        //    //if ((int) pieceType != (int)PieceType.Knight)
        //    //{
        //    //    return null;
        //    //}


        //    ///check each type of the fucking type na mean
        //    int i = (int)pieceType;
        //    switch (i)
        //    {
        //        case 0:
        //            Console.WriteLine($"Hi I am a pawn: {i}");
        //            if (piece.CanMovePawn(to, board))
        //            {
        //                board[from.X, from.Y] = null;
        //                board[to.X, to.Y] = piece;
        //                piece.SetPosition(to);

        //                if (piece.Color == PieceColor.White)
        //                {
        //                    currentPosition = to;
        //                }

        //                return to;
        //            }
        //            break;

        //        case 1:
        //            Console.WriteLine($"Hi I am a Knight: {i}");
        //            if (piece.CanMoveKnight(to, board))
        //            {
        //                Console.WriteLine("Moving from: " + from.X + "," + from.Y);
        //                board[to.X, to.Y] = piece;
        //                Console.WriteLine("Trying to move to: " + to.X + "," + to.Y); board[from.X, from.Y] = null;
        //                piece.SetPosition(to);

        //                if (piece.Color == PieceColor.White)
        //                {
        //                    currentPosition = to;
        //                }
        //                return to;
        //            }
        //            break;

        //        case 2:
        //            Console.WriteLine($"I am a bishop: {i}");
        //            break;
        //        case 3:
        //            Console.WriteLine($"I am a Rook: {i}");
        //            break;
        //        case 4:
        //            Console.WriteLine($"I am a Queen: {i}");
        //            break;
        //        case 5:
        //            Console.WriteLine($"I am a King: {i}");
        //            break;
        //    }

        //    //if (piece.CanMovePiece(to, boardToUse))
        //    //{

        //    //    boardToUse[to.X, to.Y] = piece;
        //    //    boardToUse[from.X, from.Y] = null;
        //    //    piece.SetPosition(to);

        //    //    if (piece.Color == PieceColor.White && i == 1)
        //    //    {
        //    //        currentPosition = to;
        //    //    }

        //    //    return to;
        //    //}

        //    return null;
        //}

        public MoveResult ValidateAndMovePiece(Coord from, Coord to, PieceType type, ChessPiece[,] board)
        {
            if (!IsValidCoordinate(from) || !IsValidCoordinate(to))
            {
                return new MoveResult { IsValid = false, Message = "Invalid coordinates" };
            }

            ChessPiece piece = board[from.X, from.Y];
            if (piece == null)
            {
                return new MoveResult { IsValid = false, Message = "No piece at selected position" };
            }

            if (type != piece.PieceType)
            {
                Debug.WriteLine($"Passed type is {type} ({type.GetType()})");
                Debug.WriteLine($"Actual type is {piece.PieceType} ({piece.PieceType.GetType()})");
                return new MoveResult { IsValid = false, Message = "Piece type mismatch" };
            }

            bool moved = false;

            Debug.WriteLine($"[DEBUG] Piece at ({from.X}, {from.Y}) is {piece.PieceType}");
            switch ((int)type)
            {
                case 0: // Pawn
                    Debug.WriteLine("[DEBUG] we at pawn");
                    if (piece.CanMovePawn(to, board))
                    {
                        Debug.WriteLine("[DEBUG] Attempting pawn move");
                        moved = true;
                    }
                    break;

                case 1: // Knight
                    Debug.WriteLine("[DEBUG] we at knight");
                    if (piece.CanMoveKnight(to, board))
                    {
                        Debug.WriteLine("[DEBUG] Attempting k move");
                        moved = true;
                    }
                    break;

                case 2: //Bishop
                    Debug.WriteLine("[DEBUG] we at bisherp");
                    if (piece.CanMoveBishop(to, board))
                    {
                        Debug.WriteLine("[DEBUG] Attempting b move");
                        moved = true;
                    }
                    break;
                case 3:
                    Debug.WriteLine("[DEBUG] we at rooooooooooook");
                    if (piece.CanMoveRook(to, board))
                    {
                        Debug.WriteLine("[DEBUG] Attempting R move");
                        moved = true;
                    }
                    break;
                case 4:
                    Debug.WriteLine("[DEBUG] we at QUEEN PURRR");
                    if (piece.CanMoveQueen(to, board))
                    {
                        Debug.WriteLine("[DEBUG] Attempting Q move");
                        moved = true;
                    }
                    break;
                case 5:
                    Debug.WriteLine("[DEBUG] we at KINGGGGG UHHHH");
                    if (piece.CanMoveKing(to, board))
                    {
                        Debug.WriteLine("[DEBUG] Attempting K move");
                        moved = true;
                    }
                    break;

                default:
                    return new MoveResult { IsValid = false, Message = "Unknown piece type" };
            }

            if (!moved)
            {
                Debug.WriteLine($"[DEBUG] Passed type: {type}, Actual type: {piece.PieceType}");
                return new MoveResult { IsValid = false, Message = "Invalid move for this piece" };
            }

            // Execute the move
            board[to.X, to.Y] = piece;
            board[from.X, from.Y] = null;
            piece.SetPosition(to);

            if (piece.Color == PieceColor.White)
            {
                currentPosition = to;
            }

            return new MoveResult
            {
                IsValid = true,
                Message = "Move successful",
                NewPosition = to,
                PieceColor = piece.Color.ToString(),
                PieceType = (int)piece.PieceType,
            };
        }



        public class MoveResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
            public Coord NewPosition { get; set; }
            public int PieceType { get; set; }
            public string PieceColor { get; set; }
            public object X { get; internal set; }
            public object Y { get; internal set; }
        }
    }
}

