-- Update Employee
CREATE OR ALTER PROCEDURE sp_UpdateEmployee
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
        -- Update Employee
        UPDATE Employees 
        SET Name = @Name, 
            Email = @Email, 
            JobRole = @JobRole,
            Role = @SystemRole
        WHERE Id = @Id;

        -- Update User
        UPDATE Users
        SET Username = @Name, 
            Email = @Email, 
            Role = @SystemRole
        WHERE Email = @Email;

        -- Return updated employee
        SELECT 
            e.Id,
            e.Name,
            e.Email,
            e.JobRole,
            e.Role
        FROM Employees e
        WHERE e.Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
