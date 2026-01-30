-- Get All Employees
CREATE OR ALTER PROCEDURE sp_GetAllEmployees
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
    ORDER BY e.Id;
END
GO
