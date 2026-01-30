-- Get Employee by ID
CREATE OR ALTER PROCEDURE sp_GetEmployeeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.JobRole,
        e.Role
    FROM Employees e
    WHERE e.Id = @Id;
END
GO
