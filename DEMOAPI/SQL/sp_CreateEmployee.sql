USE [TaskDb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CreateEmployee]
    @Name NVARCHAR(100),
    @Email NVARCHAR(150),
    @JobRole NVARCHAR(50),
    @SystemRole NVARCHAR(50),
    @Password NVARCHAR(255),
    @NewEmployeeId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Create Employee
        INSERT INTO Employees (Name, Email, JobRole, Role)
        VALUES (@Name, @Email, @JobRole, @SystemRole);
        
        SET @NewEmployeeId = SCOPE_IDENTITY();
        
        -- Create User with Username
        INSERT INTO Users (Username, Email, Password, Role)
        VALUES (@Name, @Email, @Password, @SystemRole);
        
        -- Return created employee
        SELECT 
            Id,
            Name,
            Email,
            JobRole,
            Role
        FROM Employees
        WHERE Id = @NewEmployeeId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO