USE [TaskDb]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeesPaged]    Script Date: 17/02/2026 09:35:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_GetEmployeesPaged]
    @PageNumber INT,
    @PageSize INT,
    @SortBy NVARCHAR(50) = 'Id',
    @SortOrder NVARCHAR(4) = 'ASC',
    @SearchTerm NVARCHAR(100) = NULL,
    @DepartmentId INT = NULL,
    @JobRole NVARCHAR(50) = NULL,
    @SystemRole NVARCHAR(50) = NULL,
    @ProjectId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH EmployeeCTE AS (
        SELECT 
            e.Id,
            e.Name,
            e.Email,
            e.JobRole,
            e.Role,
            e.DepartmentId,
            ROW_NUMBER() OVER (
                ORDER BY
                    CASE WHEN @SortBy = 'Id' AND @SortOrder = 'ASC' THEN e.Id END ASC,
                    CASE WHEN @SortBy = 'Id' AND @SortOrder = 'DESC' THEN e.Id END DESC,
                    CASE WHEN @SortBy = 'Name' AND @SortOrder = 'ASC' THEN e.Name END ASC,
                    CASE WHEN @SortBy = 'Name' AND @SortOrder = 'DESC' THEN e.Name END DESC,
                    CASE WHEN @SortBy = 'Email' AND @SortOrder = 'ASC' THEN e.Email END ASC,
                    CASE WHEN @SortBy = 'Email' AND @SortOrder = 'DESC' THEN e.Email END DESC,
                    CASE WHEN @SortBy = 'JobRole' AND @SortOrder = 'ASC' THEN e.JobRole END ASC,
                    CASE WHEN @SortBy = 'JobRole' AND @SortOrder = 'DESC' THEN e.JobRole END DESC
            ) AS RowNum
        FROM Employees e
        WHERE (@SearchTerm IS NULL 
            OR e.Name LIKE '%' + @SearchTerm + '%'
            OR e.Email LIKE '%' + @SearchTerm + '%'
            OR e.JobRole LIKE '%' + @SearchTerm + '%')
            AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
            AND (@JobRole IS NULL OR e.JobRole = @JobRole)
            AND (@SystemRole IS NULL OR e.Role = @SystemRole)
            AND (@ProjectId IS NULL OR EXISTS (
                SELECT 1 FROM EmployeeProjects ep
                WHERE ep.EmployeeId = e.Id AND ep.ProjectId = @ProjectId
            ))
    )
    SELECT 
        Id,
        Name,
        Email,
        JobRole,
        Role,
        DepartmentId,
        (SELECT COUNT(*) FROM EmployeeCTE) AS TotalCount
    FROM EmployeeCTE
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;