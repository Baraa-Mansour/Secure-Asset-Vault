using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess.Repositories;

public class UserRepo : IUserRepo
{
    private readonly string _connectionString;

    public UserRepo(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> IsUsernameOrEmailExistsAsync(string userName, string email)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"IF EXISTS ( SELECT 1 FROM Users WHERE Username = @userName OR Email = @email )
                            SELECT 1
                        ELSE
                            SELECT 0";

        await using SqlCommand cmd = new(query , connection);
        cmd.Parameters.AddWithValue("@userName", userName);
        cmd.Parameters.AddWithValue("@email", email);

        await connection.OpenAsync();

        int result = (int)(await cmd.ExecuteScalarAsync())!;

        return result > 0;
    }
    public async Task<bool> AddUserAsync(UserModel user)
    {
        await using SqlConnection connection = new(_connectionString);

        await using SqlCommand cmd = new("SP_CreateUser", connection);
        AddUserParametersToCommand(cmd, user);
        cmd.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();

        user.Id = (int)(await cmd.ExecuteScalarAsync())!;

        return true;
    }
    public async Task<UserModel?> FindUserByUsernameOrEmailAsync(string userNameOrEmail)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"SELECT * FROM Users
                         WHERE Users.Username = @userNameOrEmail OR Users.Email = @userNameOrEmail;";

        await using SqlCommand command = new(query , connection);
        command.Parameters.AddWithValue("@userNameOrEmail", userNameOrEmail);

        await connection.OpenAsync();

        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        
        if(reader.Read())
        {
            return GetModelFromReader(reader);
        }

        return null;
    }
    public async Task<UserModel?> GetUserAsync(int id)
    {
        await using SqlConnection connection = new(_connectionString);
        string query = @"SELECT * FROM Users
                         WHERE Users.Id = @id;";

        await using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();

        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        if (reader.Read())
        {
            return GetModelFromReader(reader);
        }

        return null;
    }

    public void AddUserParametersToCommand(SqlCommand cmd , UserModel user)
    {
        cmd.Parameters.AddWithValue("@userName", user.Username);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@passwordSalt", user.PasswordSalt);
        cmd.Parameters.AddWithValue("@createdAt", user.CreatedAt);
        cmd.Parameters.AddWithValue("@isActive", user.IsActive);
        cmd.Parameters.AddWithValue("@isDeleted", user.IsDeleted);
    }
    public UserModel GetModelFromReader(SqlDataReader reader)
    {
        UserModel user = new();

        int idIn = reader.GetOrdinal("Id");
        int usernameIn = reader.GetOrdinal("Username");
        int emailIn = reader.GetOrdinal("Email");
        int passwordHashIn = reader.GetOrdinal("PasswordHash");
        int passwordSaltIn = reader.GetOrdinal("PasswordSalt");
        int createdAtIn = reader.GetOrdinal("CreatedAt");
        int isActiveIn = reader.GetOrdinal("IsActive");
        int isDeletedIn = reader.GetOrdinal("IsDeleted");

        user.Id = reader.GetInt32(idIn);
        user.Username = reader.GetString(usernameIn);
        user.Email = reader.GetString(emailIn);
        user.PasswordHash = (byte[])reader.GetValue(passwordHashIn);
        user.PasswordSalt = (byte[])reader.GetValue(passwordSaltIn);
        user.CreatedAt = reader.GetDateTime(createdAtIn);
        user.IsActive = reader.GetBoolean(isActiveIn);
        user.IsDeleted = reader.GetBoolean(isDeletedIn);

        return user;
    }

}
