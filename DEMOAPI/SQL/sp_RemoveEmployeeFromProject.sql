-- Remove an employee from a project
CREATE PROCEDURE sp_RemoveEmployeeFromProject
    @EmployeeId INT,
    @ProjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if assignment exists
    IF NOT EXISTS (SELECT 1 FROM EmployeeProjects 
                   WHERE EmployeeId = @EmployeeId AND ProjectId = @ProjectId)
    BEGIN
        RETURN 0; -- Not found
    END

    -- Delete the assignment
    DELETE FROM EmployeeProjects 
    WHERE EmployeeId = @EmployeeId AND ProjectId = @ProjectId;

    RETURN 1; -- Success
END
GO
