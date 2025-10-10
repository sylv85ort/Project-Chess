namespace ChessBackend
{
    public interface IGameRepository
    {
        int StartNewGame(int user1Id, int user2Id);
        List<object> LoadGame(int gameId);
        void SaveBoardToDatabase(int gameID, List<object> boardState);
        void SaveSnapshot(int gameID, int turnNumber, List<object> boardState);
        List<List<object>> GetSnapshots(int gameID);
        int GetPlayerIdByColor(int gameID, string color);
        void DeclareGameResult(int gameId, int? winnerUserId, string gameStatus);
        string GetGameStatus(int gameId);
        (int whiteId, int blackId) GetPlayerColors(int gameId);
        int GetUserIdByGamePlayer(int gamePlayerID);
    }
}
