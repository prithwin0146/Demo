USE [TaskDb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[UpdateEmployee]
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(150),
    @JobRole NVARCHAR(50),
    @SystemRole NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Get the old email before updating
        DECLARE @OldEmail NVARCHAR(150);
        SELECT @OldEmail = Email FROM Employees WHERE Id = @Id;
        
        -- Update Employee table (including Role)
        UPDATE Employees 
        SET Name = @Name, 
            Email = @Email, 
            JobRole = @JobRole, 
            Role = @SystemRole
        WHERE Id = @Id;

        -- Update User table (sync role and other fields)
        UPDATE Users
        SET Username = @Name, 
            Email = @Email, 
            Role = @SystemRole
        WHERE Email = @OldEmail;
        
        -- Return updated employee
        SELECT 
            Id,
            Name,
            Email,
            JobRole,
            Role
        FROM Employees
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO