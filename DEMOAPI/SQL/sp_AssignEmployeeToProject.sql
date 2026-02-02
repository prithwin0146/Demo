-- Assign an employee to a project
CREATE PROCEDURE sp_AssignEmployeeToProject
    @EmployeeId INT,
    @ProjectId INT,
    @Role NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if assignment already exists
    IF EXISTS (SELECT 1 FROM EmployeeProjects 
               WHERE EmployeeId = @EmployeeId AND ProjectId = @ProjectId)
    BEGIN
        -- Return existing assignment ID
        SELECT EmployeeProjectId 
        FROM EmployeeProjects 
        WHERE EmployeeId = @EmployeeId AND ProjectId = @ProjectId;
        RETURN;
    END

    -- Insert new assignment
    INSERT INTO EmployeeProjects (EmployeeId, ProjectId, AssignedDate, Role)
    VALUES (@EmployeeId, @ProjectId, GETDATE(), @Role);

    -- Return the new ID
    SELECT SCOPE_IDENTITY() AS EmployeeProjectId;
END
GO
