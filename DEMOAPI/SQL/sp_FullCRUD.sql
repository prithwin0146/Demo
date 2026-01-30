-- Get Employee by ID
CREATE OR ALTER PROCEDURE sp_GetEmployeeById
    @Id INT
AS
BEGIN
    SELECT e.*, u.Email, u.Role 
    FROM Employees e
    LEFT JOIN Users u ON e.UserId = u.Id
    WHERE e.Id = @Id;
END
GO

-- Create Employee
CREATE OR ALTER PROCEDURE sp_CreateEmployee
    @Name NVARCHAR(100),
    @Email NVARCHAR(150),
    @JobRole NVARCHAR(50),
    @SystemRole NVARCHAR(50),
    @Password NVARCHAR(255),
    @NewEmployeeId INT OUTPUT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Create User
        DECLARE @UserId INT;
        INSERT INTO Users (Username, Email, Password, Role)
        VALUES (@Name, @Email, @Password, @SystemRole);
        SET @UserId = SCOPE_IDENTITY();

        -- Create Employee
        INSERT INTO Employees (Name, Email, JobRole, UserId)
        VALUES (@Name, @Email, @JobRole, @UserId);
        SET @NewEmployeeId = SCOPE_IDENTITY();

        -- Return created employee
        SELECT e.*, u.Email, u.Role 
        FROM Employees e
        LEFT JOIN Users u ON e.UserId = u.Id
        WHERE e.Id = @NewEmployeeId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Update Employee
CREATE OR ALTER PROCEDURE sp_UpdateEmployee
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(150),
    @JobRole NVARCHAR(50),
    @SystemRole NVARCHAR(50)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Update Employee
        UPDATE Employees 
        SET Name = @Name, Email = @Email, JobRole = @JobRole
        WHERE Id = @Id;

        -- Update User
        UPDATE Users
        SET Username = @Name, Email = @Email, Role = @SystemRole
        WHERE Id = (SELECT UserId FROM Employees WHERE Id = @Id);

        -- Return updated employee
        SELECT e.*, u.Email, u.Role 
        FROM Employees e
        LEFT JOIN Users u ON e.UserId = u.Id
        WHERE e.Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Delete Employee
CREATE OR ALTER PROCEDURE sp_DeleteEmployee
    @Id INT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @UserId INT;
        
        -- Get employee data before deletion
        SELECT e.*, u.Email, u.Role 
        FROM Employees e
        LEFT JOIN Users u ON e.UserId = u.Id
        WHERE e.Id = @Id;

        SELECT @UserId = UserId FROM Employees WHERE Id = @Id;

        -- Delete Employee
        DELETE FROM Employees WHERE Id = @Id;

        -- Delete User
        IF @UserId IS NOT NULL
            DELETE FROM Users WHERE Id = @UserId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO