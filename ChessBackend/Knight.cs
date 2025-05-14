//namespace ChessBackend
//{
//    public class Knight : ChessPiece
//    {
//        //public Knight(PieceColor color) : base(color)
//        //{
//        //}

//        //public override bool CanMovePiece(Coord to, ChessPiece[,] board)
//        //{
//        //    int dx = to.X - CurrentPosition.X;
//        //    int dy = to.Y - CurrentPosition.Y;

//        //    // L-shape movement: 2 squares in one direction and 1 square perpendicular
//        //    bool isLShape = (Math.Abs(dx) == 2 && Math.Abs(dy) == 1) ||
//        //                   (Math.Abs(dx) == 1 && Math.Abs(dy) == 2);

//        //    if (!isLShape) return false;

//        //    // Check if target square is empty or has an enemy piece
//        //    ChessPiece target = board[to.X, to.Y];
//        //    if (target != null && target.Color == this.Color)
//        //        return false;  // Can't capture own piece

//        //    return true;
//        //}
//    }
//}