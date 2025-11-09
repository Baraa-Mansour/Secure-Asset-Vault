using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess.Repositories;

public class VaultRepo : IVaultRepo
{
    private readonly string _connectionString;

    public VaultRepo(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<List<VaultModel>> GetVaultsByUserIdAsync(int userId)
    {
        List<VaultModel> vaults = new();

        await using SqlConnection connection = new(_connectionString);
        string query = @"SELECT * FROM Vaults WHERE UserId = @userId";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@userId", userId);

        await connection.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (reader.Read())
        {
            vaults.Add(MapReaderToModel(reader));
        }

        return vaults;
    }
    public async Task<VaultModel?> GetVaultsByIdAsync(int vaultId)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"SELECT * FROM Vaults WHERE Id = @vaultId";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@vaultId", vaultId);

        await connection.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (reader.Read())
        {
            return MapReaderToModel(reader);
        }

        return null;
    }
    public async Task<bool> CreateVaultByUserIdAsync(VaultModel vault)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"INSERT INTO Vaults (UserId, Name, Description, CreatedAt)
                         OUTPUT INSERTED.Id
                         VALUES (@userId, @name, @description, @createdAt)";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@userId", vault.UserId);
        cmd.Parameters.AddWithValue("@name", vault.Name);
        cmd.Parameters.AddWithValue("@description", (object?)vault.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@createdAt", vault.CreatedAt);

        await connection.OpenAsync();
        vault.Id = (int)(await cmd.ExecuteScalarAsync())!;
        return true;
    }
    public async Task<bool> UpdateVaultAsync(VaultModel vault)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"UPDATE Vaults
                         SET Name = @name, Description = @description
                         WHERE Id = @vaultId";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@name", vault.Name);
        cmd.Parameters.AddWithValue("@description", (object?)vault.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@vaultId", vault.Id);

        await connection.OpenAsync();
        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }
    public async Task<bool> DeleteVaultAsync(int vaultId)
    {
        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand cmd = new("SP_DeleteVaultAndItems", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@vaultId", vaultId);

        await connection.OpenAsync();
        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }


    private VaultModel MapReaderToModel(SqlDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("Id");
        int userIdOrdinal = reader.GetOrdinal("UserId");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descOrdinal = reader.GetOrdinal("Description");
        int createdAtOrdinal = reader.GetOrdinal("CreatedAt");

        return new VaultModel
        {
            Id = reader.GetInt32(idOrdinal),
            UserId = reader.GetInt32(userIdOrdinal),
            Name = reader.GetString(nameOrdinal),
            Description = reader.IsDBNull(descOrdinal) ? null : reader.GetString(descOrdinal),
            CreatedAt = reader.GetDateTime(createdAtOrdinal)
        };
    }
}

