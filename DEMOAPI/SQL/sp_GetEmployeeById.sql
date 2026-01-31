-- Get Employee By ID
CREATE OR ALTER PROCEDURE sp_GetEmployeeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        JobRole,
        Role
    FROM Employees
    WHERE Id = @Id;
END
GO