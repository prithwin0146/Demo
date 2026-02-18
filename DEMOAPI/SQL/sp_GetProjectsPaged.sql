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
    @SearchTerm NVARCHAR(100) = NULL,
    @HasEmployeesOnly BIT = 0,
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    IF @HasEmployeesOnly = 1
    BEGIN
        -- INNER JOIN: Only projects WITH assigned employees
        WITH ProjectCTE AS (
            SELECT 
                p.ProjectId,
                p.ProjectName,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Status,
                COUNT(DISTINCT ep.EmployeeId) AS AssignedEmployees,
                STRING_AGG(e.Name, ', ') WITHIN GROUP (ORDER BY e.Name) AS EmployeeNames,
                ROW_NUMBER() OVER (
                    ORDER BY
                        CASE WHEN @SortBy = 'ProjectId' AND @SortOrder = 'ASC' THEN p.ProjectId END ASC,
                        CASE WHEN @SortBy = 'ProjectId' AND @SortOrder = 'DESC' THEN p.ProjectId END DESC,
                        CASE WHEN @SortBy = 'ProjectName' AND @SortOrder = 'ASC' THEN p.ProjectName END ASC,
                        CASE WHEN @SortBy = 'ProjectName' AND @SortOrder = 'DESC' THEN p.ProjectName END DESC,
                        CASE WHEN @SortBy = 'StartDate' AND @SortOrder = 'ASC' THEN p.StartDate END ASC,
                        CASE WHEN @SortBy = 'StartDate' AND @SortOrder = 'DESC' THEN p.StartDate END DESC,
                        CASE WHEN @SortBy = 'Status' AND @SortOrder = 'ASC' THEN p.Status END ASC,
                        CASE WHEN @SortBy = 'Status' AND @SortOrder = 'DESC' THEN p.Status END DESC
                ) AS RowNum
            FROM Projects p
            INNER JOIN EmployeeProjects ep ON p.ProjectId = ep.ProjectId
            INNER JOIN Employees e ON ep.EmployeeId = e.Id
            WHERE (@SearchTerm IS NULL 
                OR p.ProjectName LIKE '%' + @SearchTerm + '%'
                OR p.Description LIKE '%' + @SearchTerm + '%'
                OR e.Name LIKE '%' + @SearchTerm + '%')
                AND (@Status IS NULL OR p.Status = @Status)
            GROUP BY 
                p.ProjectId,
                p.ProjectName,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Status
        )
        SELECT 
            ProjectId, ProjectName, Description, StartDate, EndDate, Status,
            AssignedEmployees, EmployeeNames,
            (SELECT COUNT(*) FROM ProjectCTE) AS TotalCount
        FROM ProjectCTE
        WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
        ORDER BY RowNum;
    END
    ELSE
    BEGIN
        -- LEFT JOIN: All projects including those with no employees
        WITH ProjectCTE AS (
            SELECT 
                p.ProjectId,
                p.ProjectName,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Status,
                COUNT(DISTINCT ep.EmployeeId) AS AssignedEmployees,
                STRING_AGG(e.Name, ', ') WITHIN GROUP (ORDER BY e.Name) AS EmployeeNames,
                ROW_NUMBER() OVER (
                    ORDER BY
                        CASE WHEN @SortBy = 'ProjectId' AND @SortOrder = 'ASC' THEN p.ProjectId END ASC,
                        CASE WHEN @SortBy = 'ProjectId' AND @SortOrder = 'DESC' THEN p.ProjectId END DESC,
                        CASE WHEN @SortBy = 'ProjectName' AND @SortOrder = 'ASC' THEN p.ProjectName END ASC,
                        CASE WHEN @SortBy = 'ProjectName' AND @SortOrder = 'DESC' THEN p.ProjectName END DESC,
                        CASE WHEN @SortBy = 'StartDate' AND @SortOrder = 'ASC' THEN p.StartDate END ASC,
                        CASE WHEN @SortBy = 'StartDate' AND @SortOrder = 'DESC' THEN p.StartDate END DESC,
                        CASE WHEN @SortBy = 'Status' AND @SortOrder = 'ASC' THEN p.Status END ASC,
                        CASE WHEN @SortBy = 'Status' AND @SortOrder = 'DESC' THEN p.Status END DESC
                ) AS RowNum
            FROM Projects p
            LEFT JOIN EmployeeProjects ep ON p.ProjectId = ep.ProjectId
            LEFT JOIN Employees e ON ep.EmployeeId = e.Id
            WHERE (@SearchTerm IS NULL 
                OR p.ProjectName LIKE '%' + @SearchTerm + '%'
                OR p.Description LIKE '%' + @SearchTerm + '%'
                OR e.Name LIKE '%' + @SearchTerm + '%')
                AND (@Status IS NULL OR p.Status = @Status)
            GROUP BY 
                p.ProjectId,
                p.ProjectName,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Status
        )
        SELECT 
            ProjectId, ProjectName, Description, StartDate, EndDate, Status,
            AssignedEmployees, EmployeeNames,
            (SELECT COUNT(*) FROM ProjectCTE) AS TotalCount
        FROM ProjectCTE
        WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
        ORDER BY RowNum;
    END
END;
GO