CREATE PROCEDURE SP_CreateUser
	@userName NVARCHAR(100),
	@email NVARCHAR(255),
	@passwordHash VARBINARY(MAX),
	@passwordSalt VARBINARY(MAX),
	@createdAt DATETIME2(7),
	@isActive BIT,
	@isDeleted BIT
AS
BEGIN
	INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, CreatedAt, IsActive, IsDeleted)
	VALUES (@userName , @email , @passwordHash , @passwordSalt,
			@createdAt , @isActive , @isDeleted);
	SELECT CAST(SCOPE_IDENTITY() AS INT);
END