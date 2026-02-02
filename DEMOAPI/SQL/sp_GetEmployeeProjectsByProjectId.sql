-- Get all employee assignments for a specific project
CREATE PROCEDURE sp_GetEmployeeProjectsByProjectId
    @ProjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ep.EmployeeProjectId,
        ep.EmployeeId,
        e.Name AS EmployeeName,
        ep.ProjectId,
        ep.AssignedDate,
        ep.Role
    FROM EmployeeProjects ep
    INNER JOIN Employees e ON ep.EmployeeId = e.Id
    WHERE ep.ProjectId = @ProjectId
    ORDER BY ep.AssignedDate DESC;
END
GO
