
namespace ChessBackend
{
    public class GameService
    {
        private readonly GameEngine _gameEngine;
        private readonly IGameRepository _repo;

        public GameService(GameEngine gameEngine, IGameRepository repo)
        {
            _gameEngine = gameEngine;
            _repo = repo;
        }

        public int StartNewGame(int user1Id, int user2Id) => _repo.StartNewGame(user1Id, user2Id);
        public List<object> LoadGame(int gameId) => _repo.LoadGame(gameId);
        public void SaveBoardToDatabase(int gameID, List<object> boardState) => _repo.SaveBoardToDatabase(gameID, boardState);
        public void SaveSnapshot(int gameID, int turnNumber, List<object> boardState) => _repo.SaveSnapshot(gameID, turnNumber, boardState);
        public List<List<object>> GetSnapshots(int gameID) => _repo.GetSnapshots(gameID);
        public int GetPlayerIdByColor(int gameID, string color) => _repo.GetPlayerIdByColor(gameID, color);
        public void DeclareGameResult(int gameId, int? winnerUserId, string gameStatus) => _repo.DeclareGameResult(gameId, winnerUserId, gameStatus);
        public string GetGameStatus(int gameId) => _repo.GetGameStatus(gameId);
        public (int whiteId, int blackId) GetPlayerColors(int gameId) => _repo.GetPlayerColors(gameId);
        public int GetUserIdByGamePlayer(int gamePlayerID) => _repo.GetUserIdByGamePlayer(gamePlayerID);

        public object GetBoardState()
        {
            var boardState = new List<object>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var piece = _gameEngine.Board[x, y];
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
