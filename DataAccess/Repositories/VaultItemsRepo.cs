using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Data.SqlClient;

namespace DataAccess.Repositories;

public class VaultItemsRepo : IVaultItemsRepo
{
    private readonly string _connectionString;

    public VaultItemsRepo(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<VaultItemModel>> GetVaultItemsAsync(int vaultId)
    {
        List<VaultItemModel> items = new();

        await using SqlConnection connection = new(_connectionString);
        string query = @"SELECT * FROM VaultItems WHERE VaultId = @vaultId";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@vaultId", vaultId);

        await connection.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (reader.Read())
        {
            items.Add(MapReaderToModel(reader));
        }

        return items;
    }
    public async Task<VaultItemModel?> GetItemByIdAsync(int itemId)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"SELECT * FROM VaultItems WHERE Id = @itemId";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@itemId", itemId);

        await connection.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (reader.Read())
        {
            return MapReaderToModel(reader);
        }

        return null;
    }
    public async Task<bool> CreateVaultItemAsync(VaultItemModel item)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"INSERT INTO VaultItems (VaultId, Alias, EncryptedData, EncryptedKey_ForUser, IV, CreatedAt, UpdatedAt)
                         OUTPUT INSERTED.Id
                         VALUES (@vaultId, @alias, @encryptedData, @encryptedKey, @iv, @createdAt, @updatedAt)";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@vaultId", item.VaultId);
        cmd.Parameters.AddWithValue("@alias", item.Alias);
        cmd.Parameters.AddWithValue("@encryptedData", item.EncryptedData);
        cmd.Parameters.AddWithValue("@encryptedKey", item.EncryptedKey_ForUser);
        cmd.Parameters.AddWithValue("@iv", item.IV);
        cmd.Parameters.AddWithValue("@createdAt", item.CreatedAt);
        cmd.Parameters.AddWithValue("@updatedAt", item.UpdatedAt);

        await connection.OpenAsync();
        item.Id = (int)(await cmd.ExecuteScalarAsync())!;
        return true;
    }
    public async Task<bool> DeleteItemAsync(int itemId)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"DELETE FROM VaultItems WHERE Id = @itemId";

        await using SqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@itemId", itemId);

        await connection.OpenAsync();
        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private VaultItemModel MapReaderToModel(SqlDataReader reader)
    {
        return new VaultItemModel
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            VaultId = reader.GetInt32(reader.GetOrdinal("VaultId")),
            Alias = reader.GetString(reader.GetOrdinal("Alias")),
            EncryptedData = (byte[])reader.GetValue(reader.GetOrdinal("EncryptedData")),
            EncryptedKey_ForUser = (byte[])reader.GetValue(reader.GetOrdinal("EncryptedKey_ForUser")),
            IV = (byte[])reader.GetValue(reader.GetOrdinal("IV")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }
}

