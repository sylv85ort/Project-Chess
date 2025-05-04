using System.Diagnostics;

namespace ChessBackend
{
    public class GameService
    {
        public Coord knightPosition { get; private set; } = new Coord(2, 5);
        public Coord currentPosition;
        public Coord lastSelectedPosition;

        public GameService()
        {
            currentPosition = knightPosition;
        }
        public Coord ValidateAndMoveKnight(Coord newPosition)

        {
            Debug.WriteLine("validating");

            if (CanMoveKnight(newPosition))
                {
                    Debug.WriteLine("new position: " + newPosition);
                    currentPosition = newPosition;
                    return currentPosition;

                }
                Debug.WriteLine("sfnjk");
                return null; // Invalid move
        }

        public Coord LastSelectedPiece(Coord lastSelectedPos)
        {
            if (lastSelectedPos == knightPosition)
            {
                Debug.WriteLine($"Is Selected Piece?: {lastSelectedPos.X}, {lastSelectedPos.Y}");
                knightPosition = lastSelectedPos;
                lastSelectedPos = lastSelectedPosition;
                return lastSelectedPos;
            }
            else
            {
                Debug.WriteLine($"No piece found at: {lastSelectedPos.X}, {lastSelectedPos.Y}");
                return null;
            }
        }


        public bool CanMoveKnight(Coord to)
        {
            var dx = to.X - currentPosition.X;
            var dy = to.Y - currentPosition.Y;

            return (Math.Abs(dx) == 2 && Math.Abs(dy) == 1) ||
                   (Math.Abs(dx) == 1 && Math.Abs(dy) == 2);
        }
    }
}
