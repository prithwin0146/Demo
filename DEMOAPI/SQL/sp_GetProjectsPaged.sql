-- =============================================
-- Project Pagination with CTE
-- =============================================

IF OBJECT_ID('sp_GetProjectsPaged', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetProjectsPaged;
GO

CREATE PROCEDURE sp_GetProjectsPaged
    @PageNumber INT,
    @PageSize INT,
    @SortBy NVARCHAR(50) = 'ProjectName',
    @SortOrder NVARCHAR(4) = 'ASC',
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH ProjectCTE AS (
        SELECT 
            p.ProjectId,
            p.ProjectName,
            p.Description,
            p.StartDate,
            p.EndDate,
            p.Status,
            (SELECT COUNT(*) FROM EmployeeProjects WHERE ProjectId = p.ProjectId) AS AssignedEmployees,
            ROW_NUMBER() OVER (
                ORDER BY
                    CASE WHEN @SortBy = 'ProjectId' AND @SortOrder = 'ASC' THEN p.ProjectId END ASC,
                    CASE WHEN @SortBy = 'ProjectId' AND @SortOrder = 'DESC' THEN p.ProjectId END DESC,
                    CASE WHEN @SortBy = 'ProjectName' AND @SortOrder = 'ASC' THEN p.ProjectName END ASC,
                    CASE WHEN @SortBy = 'ProjectName' AND @SortOrder = 'DESC' THEN p.ProjectName END DESC,
                    CASE WHEN @SortBy = 'StartDate' AND @SortOrder = 'ASC' THEN p.StartDate END ASC,
                    CASE WHEN @SortBy = 'StartDate' AND @SortOrder = 'DESC' THEN p.StartDate END DESC
            ) AS RowNum
        FROM Projects p
        WHERE (@SearchTerm IS NULL 
            OR p.ProjectName LIKE '%' + @SearchTerm + '%'
            OR p.Description LIKE '%' + @SearchTerm + '%')
    )
    SELECT 
        ProjectId,
        ProjectName,
        Description,
        StartDate,
        EndDate,
        Status,
        AssignedEmployees,
        (SELECT COUNT(*) FROM ProjectCTE) AS TotalCount
    FROM ProjectCTE
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO

PRINT 'sp_GetProjectsPaged created successfully!';
GO
