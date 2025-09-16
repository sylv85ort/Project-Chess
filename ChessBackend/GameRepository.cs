using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;

namespace ChessBackend
{
    public class GameRepository
    {
        private readonly IConfiguration _configuration;

        public GameRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString() => _configuration.GetConnectionString("DefaultConnection");

        public async Task<int> StartNewGameAsync(int user1Id, int user2Id)
        {

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand("StartNewGame", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@User1ID", user1Id);
                cmd.Parameters.AddWithValue("@User2ID", user2Id);

                SqlParameter outputParam = new SqlParameter("@GameID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                connection.Open();
                cmd.ExecuteNonQuery();
                return (int)outputParam.Value;
            }
        }

        public List<object> LoadGame(int gameId)
        {
            var pieces = new List<object>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("LoadGame", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GameID", gameId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pieces.Add(new
                            {
                                position = new
                                {
                                    x = reader.GetInt32(reader.GetOrdinal("squareX")),
                                    y = reader.GetInt32(reader.GetOrdinal("squareY"))
                                },
                                pieceType = reader.GetInt32(reader.GetOrdinal("pieceTypeID")),
                                pieceColor = reader.GetString(reader.GetOrdinal("player_color"))
                            });
                        }
                    }
                }
            }

            return pieces;
        }

        public void SaveBoardToDatabase(int gameID, List<object> boardState)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SaveFullBoardState", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@gameID", gameID);


                    var pieceTable = new DataTable();
                    pieceTable.Columns.Add("squareX", typeof(int));
                    pieceTable.Columns.Add("squareY", typeof(int));
                    pieceTable.Columns.Add("pieceTypeID", typeof(int));
                    pieceTable.Columns.Add("gamePlayerID", typeof(int));
                    pieceTable.Columns.Add("isCaptured", typeof(bool));

                    foreach (var item in boardState)
                    {
                        dynamic piece = item;
                        int playerIdForPiece = GetPlayerIdByColor(gameID, piece.pieceColor);

                        pieceTable.Rows.Add(
                            piece.position.x,
                            piece.position.y,
                            piece.pieceType,
                            playerIdForPiece,
                            false
                        );
                    }


                    var param = cmd.Parameters.AddWithValue("@Pieces", pieceTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "dbo.PieceTableType";

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveSnapshot(int gameID, int turnNumber, List<object> boardState)
        {
            string boardJson = JsonSerializer.Serialize<object>(boardState);

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SaveBoardSnapshot", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@gameID", gameID);
                    cmd.Parameters.AddWithValue("@turnNumber", turnNumber);
                    cmd.Parameters.AddWithValue("@boardState", boardJson);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<List<object>> GetSnapshots(int gameID)
        {
            List<List<object>> boardStates = new List<List<object>>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT boardState FROM GameBoardSnapshots WHERE gameID = @gameID ORDER BY turnNumber", connection))
                {
                    cmd.Parameters.AddWithValue("@gameID", gameID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string json = reader.GetString(0);
                            var snapshot = JsonSerializer.Deserialize<List<object>>(json);
                            boardStates.Add(snapshot);
                        }
                    }

                    return (boardStates);
                }
            }
        }


        public int GetPlayerIdByColor(int gameID, string color)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(
                    "SELECT gamePlayerID FROM GamePlayers WHERE gameID = @gameID AND player_color = @color", connection))
                {
                    cmd.Parameters.AddWithValue("@gameID", gameID);
                    cmd.Parameters.AddWithValue("@color", color);

                    var result = cmd.ExecuteScalar();
                    return result != null ? (int)result : -1;
                }
            }
        }

        public void DeclareGameResult(int gameId, int? winnerUserId, string gameStatus)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            using (var command = new SqlCommand("DeclareGameResult", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@GameId", gameId);

                if (winnerUserId.HasValue)
                {
                    command.Parameters.AddWithValue("@WinnerUserId", winnerUserId.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@WinnerUserId", DBNull.Value);
                }

                command.Parameters.AddWithValue("@gameStatus", gameStatus);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public string GetGameStatus(int gameId)
        {
            using var conn = new SqlConnection(GetConnectionString());
            using var cmd = new SqlCommand("SELECT gameStatus FROM Games WHERE gameID = @id", conn);
            cmd.Parameters.AddWithValue("@id", gameId);
            conn.Open();
            var val = cmd.ExecuteScalar() as string;
            return string.IsNullOrEmpty(val) ? "In Progress" : val;
        }



        public (int whiteId, int blackId) GetPlayerColors(int gameId)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                string sql = "SELECT userID, player_color FROM GamePlayers WHERE gameID = @gameID";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@gameID", gameId);

                    int whiteId = 0;
                    int blackId = 0;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            string color = reader.GetString(1);

                            if (color == "White") whiteId = userId;
                            else if (color == "Black") blackId = userId;
                        }
                    }

                    return (whiteId, blackId);
                }
            }
        }

        public int GetUserIdByGamePlayer(int gamePlayerID)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT userID FROM GamePlayers WHERE gamePlayerID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", gamePlayerID);
                    var result = cmd.ExecuteScalar();
                    return result != null ? (int)result : -1;
                }
            }
        }
    }
}
