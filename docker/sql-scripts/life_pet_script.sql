-- Criar banco de dados somente se ele não existir.
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'life_pet')
BEGIN
    CREATE DATABASE life_pet;
END
GO

USE life_pet;
GO

-- Criar tabela somente se ela não existir.
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_users]') AND type in (N'U'))
BEGIN
    CREATE TABLE tbl_users
    (
        id            INT          NOT NULL PRIMARY KEY,
        first_name    VARCHAR(50)  NOT NULL,
        last_name     VARCHAR(50)  NOT NULL,
        email         VARCHAR(100) NOT NULL UNIQUE,
        username      VARCHAR(50)  NOT NULL UNIQUE,
        password_hash VARCHAR(255) NOT NULL,
        role_id       INT          NOT NULL,
        state         INT          NOT NULL DEFAULT 1
    )
END
GO

CREATE OR ALTER PROCEDURE SP_LIST_USERS
AS
BEGIN
    SELECT
        id AS Id,
        first_name AS FirstName,
        last_name AS LastName,
        email AS Email,
        username AS Username,
        password_hash AS PasswordHash,
        role_id AS RoleId,
        state AS State
    FROM tbl_users;
END
GO

CREATE OR ALTER PROCEDURE SP_CREATE_USER(
    @p_id INT = 0,
    @p_first_name VARCHAR(50),
    @p_last_name VARCHAR(50),
    @p_email VARCHAR(100),
    @p_username VARCHAR(50),
    @p_password_hash VARCHAR(255),
    @p_role_id INT
)
AS
BEGIN
    DECLARE @v_id INT;
    
    IF EXISTS(SELECT * FROM tbl_users WHERE username = @p_username) BEGIN
        THROW 60001, N'Username já cadastrado', 1;
    END

    IF @p_id = 0
        BEGIN
            SELECT @v_id = MAX(id) FROM tbl_users;
            SET @v_id = ISNULL(@v_id, 0) + 1;

            INSERT INTO tbl_users (id, first_name, last_name, email, username, password_hash, role_id, state)
            VALUES (@v_id, @p_first_name, @p_last_name, @p_email,
                    @p_username,
                    CONVERT(varchar(255),
                            HASHBYTES('SHA2_256', iif(isnull(@p_password_hash, '') = '', '12345678',
                                                      @p_password_hash)), 2), @p_role_id, 1);
        END
    ELSE
        BEGIN
            SET @v_id = @p_id;
            UPDATE tbl_users
            SET first_name    = @p_first_name,
                last_name     = @p_last_name,
                email         = @p_email,
                username      = @p_username,
                password_hash = CONVERT(varchar(255),
                        HASHBYTES('SHA2_256', iif(isnull(@p_password_hash, 1) = 1, '12345678',
                                                  @p_password_hash)), 2),
                role_id       = @p_role_id
            WHERE id = @v_id;
        END
    SELECT @v_id AS Id;
END
GO

-- Create admin user only if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM tbl_users WHERE username = 'admin')
BEGIN
    EXEC SP_CREATE_USER
        @p_first_name    = 'Jane',
        @p_last_name     = 'Smith',
        @p_email         = 'capulet@live.com',
        @p_username      = 'admin',
        @p_password_hash = '3?OfC',
        @p_role_id       = 1;
END
GO
    
CREATE OR ALTER PROCEDURE SP_DELETE_USER(
    @p_id INT
)
AS
BEGIN
    DELETE FROM tbl_users
    WHERE id = @p_id;
    
    SELECT @p_id AS Id;
END
GO

CREATE OR ALTER PROCEDURE SP_GET_USER_BY_USERNAME(
    @p_username VARCHAR(50)
)
AS
BEGIN
    SELECT
        id AS Id,
        first_name AS FirstName,
        last_name AS LastName,
        email AS Email,
        username AS Username,
        password_hash AS PasswordHash,
        role_id AS RoleId,
        state AS State
    FROM tbl_users 
    WHERE username = @p_username;
END
GO

-- Execute SP_GET_USER_BY_USERNAME only if admin user exists
IF EXISTS (SELECT 1 FROM tbl_users WHERE username = 'admin')
BEGIN
    EXECUTE SP_GET_USER_BY_USERNAME @p_username = 'admin';
END
GO

-- EXEC SP_DELETE_USER @p_id = 1;

