CREATE PROCEDURE GetProjectById
    @ProjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ProjectId,
        ProjectName,
        Description,
        StartDate,
        EndDate,
        Status
    FROM Projects
    WHERE ProjectId = @ProjectId;
END
