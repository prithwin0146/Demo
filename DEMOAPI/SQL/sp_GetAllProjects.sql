CREATE PROCEDURE GetAllProjects
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
    ORDER BY ProjectId;
END
