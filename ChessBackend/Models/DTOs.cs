namespace ChessBackend.DTOs
{
        public class CreateGameRequest
        {
            public int Player1Id { get; set; }
            public int Player2Id { get; set; }
        }

        public class CreateReplayRequest
        {
            public int GameId { get; set; }
        }

        public class LegalMoveRequest
        {
            public int GameId { get; set; }
            public Coord From { get; set; }
        }

        public class MoveRequest
        {
            public Coord From { get; set; }
            public Coord To { get; set; }
            public PieceType PieceType { get; set; }
            public PieceColor PieceColor { get; set; }
            public int UserId { get; set; }
            public int GameId { get; set; }
            public int turnNumber { get; set; }
        }

        public class GameResponse
        {
            public int GameId { get; set; }
            public int WhitePlayerId { get; set; }
            public int BlackPlayerId { get; set; }
        }

        public class SwitchUserRequest
        {
            public int gameId { get; set; }
            public int CurrentUserId { get; set; }
        }

}
