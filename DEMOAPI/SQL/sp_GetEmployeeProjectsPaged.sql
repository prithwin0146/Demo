-- =============================================
-- EmployeeProject Pagination with CTE
-- =============================================

IF OBJECT_ID('sp_GetEmployeeProjectsPaged', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetEmployeeProjectsPaged;
GO

CREATE PROCEDURE sp_GetEmployeeProjectsPaged
    @ProjectId INT,
    @PageNumber INT,
    @PageSize INT,
    @SortBy NVARCHAR(50) = 'AssignedDate',
    @SortOrder NVARCHAR(4) = 'DESC',
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH EmployeeProjectCTE AS (
        SELECT
            ep.EmployeeProjectId,
            ep.EmployeeId,
            e.Name AS EmployeeName,
            e.Email AS EmployeeEmail,
            e.JobRole AS EmployeeJobRole,
            ep.ProjectId,
            p.ProjectName,
            ep.AssignedDate,
            ep.Role,
            ROW_NUMBER() OVER (
                ORDER BY
                    CASE WHEN @SortBy = 'EmployeeProjectId' AND @SortOrder = 'ASC' THEN ep.EmployeeProjectId END ASC,
                    CASE WHEN @SortBy = 'EmployeeProjectId' AND @SortOrder = 'DESC' THEN ep.EmployeeProjectId END DESC,
                    CASE WHEN @SortBy = 'AssignedDate' AND @SortOrder = 'ASC' THEN ep.AssignedDate END ASC,
                    CASE WHEN @SortBy = 'AssignedDate' AND @SortOrder = 'DESC' THEN ep.AssignedDate END DESC,
                    CASE WHEN @SortBy = 'EmployeeName' AND @SortOrder = 'ASC' THEN e.Name END ASC,
                    CASE WHEN @SortBy = 'EmployeeName' AND @SortOrder = 'DESC' THEN e.Name END DESC,
                    CASE WHEN @SortBy = 'Role' AND @SortOrder = 'ASC' THEN ep.Role END ASC,
                    CASE WHEN @SortBy = 'Role' AND @SortOrder = 'DESC' THEN ep.Role END DESC
            ) AS RowNum
        FROM EmployeeProjects ep
        INNER JOIN Employees e ON ep.EmployeeId = e.Id
        INNER JOIN Projects p ON ep.ProjectId = p.ProjectId
        WHERE ep.ProjectId = @ProjectId
            AND (@SearchTerm IS NULL
                OR e.Name LIKE '%' + @SearchTerm + '%'
                OR ep.Role LIKE '%' + @SearchTerm + '%'
                OR e.Email LIKE '%' + @SearchTerm + '%')
    )
    SELECT
        EmployeeProjectId,
        EmployeeId,
        EmployeeName,
        EmployeeEmail,
        EmployeeJobRole,
        ProjectId,
        ProjectName,
        AssignedDate,
        Role,
        (SELECT COUNT(*) FROM EmployeeProjectCTE) AS TotalCount
    FROM EmployeeProjectCTE
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO


