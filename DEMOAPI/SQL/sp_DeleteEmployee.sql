-- Delete Employee
CREATE OR ALTER PROCEDURE sp_DeleteEmployee
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Email NVARCHAR(150);
        
        -- Get employee data before deletion
        SELECT 
            @Email = e.Email
        FROM Employees e
        WHERE e.Id = @Id;

        -- Return employee before deletion
        SELECT 
            e.Id,
            e.Name,
            e.Email,
            e.JobRole,
            e.Role
        FROM Employees e
        WHERE e.Id = @Id;

        -- Delete Employee
        DELETE FROM Employees WHERE Id = @Id;

        -- Delete User
        IF @Email IS NOT NULL
            DELETE FROM Users WHERE Email = @Email;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
