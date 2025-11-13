using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Data.SqlClient;

namespace DataAccess.Repositories;

public class AuditLogsRepo : IAuditLogsRepo
{
    private readonly string _connectionString;

    public AuditLogsRepo(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddLogAsync(AuditLogsModel logsModel)
    {
        const string query = @"INSERT INTO AuditLogs (UserId, Action, ResourceId, Message, IpAddress, TimeStamp, Severity)
                               VALUES (@UserId, @Action, @ResourceId, @Message, @IpAddress, @TimeStamp, @Severity)";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@UserId", logsModel.UserId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Action", logsModel.Action);
        command.Parameters.AddWithValue("@ResourceId", logsModel.ResourceId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Message", logsModel.Message ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@IpAddress", logsModel.IpAddress ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@TimeStamp", logsModel.TimeStamp);
        command.Parameters.AddWithValue("@Severity", logsModel.Severity);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
    public async Task ClearOldLogsAsync(DateTime wantedDate)
    {
        const string query = @"DELETE FROM AuditLogs 
                               WHERE TimeStamp < @CutoffDate";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@CutoffDate", wantedDate);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
}

