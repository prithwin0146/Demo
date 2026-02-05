
USE [TaskDb];  

-- =============================================
-- 1. UPDATE sp_GetEmployeesPaged (Pagination with Department Filter)
-- =============================================
IF OBJECT_ID('sp_GetEmployeesPaged', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetEmployeesPaged;
GO

CREATE PROCEDURE sp_GetEmployeesPaged
    @PageNumber INT,
    @PageSize INT,
    @SortBy NVARCHAR(50) = 'Id',
    @SortOrder NVARCHAR(4) = 'ASC',
    @SearchTerm NVARCHAR(100) = NULL,
    @DepartmentId INT = NULL
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
                    CASE WHEN @SortBy = 'Email' AND @SortOrder = 'DESC' THEN e.Email END DESC
            ) AS RowNum
        FROM Employees e
        WHERE (@SearchTerm IS NULL 
            OR e.Name LIKE '%' + @SearchTerm + '%'
            OR e.Email LIKE '%' + @SearchTerm + '%'
            OR e.JobRole LIKE '%' + @SearchTerm + '%')
            AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
    )
    SELECT 
        Id,
        Name,
        Email,
        JobRole,
        Role,
        DepartmentId,
        (SELECT COUNT(*) FROM Employees e
         WHERE (@SearchTerm IS NULL 
            OR e.Name LIKE '%' + @SearchTerm + '%'
            OR e.Email LIKE '%' + @SearchTerm + '%'
            OR e.JobRole LIKE '%' + @SearchTerm + '%')
            AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)) AS TotalCount
    FROM EmployeeCTE
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO

-- =============================================
-- 2. CREATE sp_GetDepartmentsPaged (Department Pagination)
-- =============================================
IF OBJECT_ID('sp_GetDepartmentsPaged', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDepartmentsPaged;
GO

CREATE PROCEDURE sp_GetDepartmentsPaged
    @PageNumber INT,
    @PageSize INT,
    @SortBy NVARCHAR(50) = 'DepartmentName',
    @SortOrder NVARCHAR(4) = 'ASC',
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH DepartmentCTE AS (
        SELECT 
            d.DepartmentId,
            d.DepartmentName,
            d.Description,
            d.ManagerId,
            e.Name AS ManagerName,
            (SELECT COUNT(*) FROM Employees WHERE DepartmentId = d.DepartmentId) AS EmployeeCount,
            ROW_NUMBER() OVER (
                ORDER BY
                    CASE WHEN @SortBy = 'DepartmentId' AND @SortOrder = 'ASC' THEN d.DepartmentId END ASC,
                    CASE WHEN @SortBy = 'DepartmentId' AND @SortOrder = 'DESC' THEN d.DepartmentId END DESC,
                    CASE WHEN @SortBy = 'DepartmentName' AND @SortOrder = 'ASC' THEN d.DepartmentName END ASC,
                    CASE WHEN @SortBy = 'DepartmentName' AND @SortOrder = 'DESC' THEN d.DepartmentName END DESC
            ) AS RowNum
        FROM Departments d
        LEFT JOIN Employees e ON d.ManagerId = e.Id
        WHERE (@SearchTerm IS NULL 
            OR d.DepartmentName LIKE '%' + @SearchTerm + '%'
            OR d.Description LIKE '%' + @SearchTerm + '%')
    )
    SELECT 
        DepartmentId,
        DepartmentName,
        Description,
        ManagerId,
        ManagerName,
        EmployeeCount,
        (SELECT COUNT(*) FROM Departments d
         WHERE (@SearchTerm IS NULL 
            OR d.DepartmentName LIKE '%' + @SearchTerm + '%'
            OR d.Description LIKE '%' + @SearchTerm + '%')) AS TotalCount
    FROM DepartmentCTE
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO

-- =============================================
-- 3. CREATE sp_GetProjectsPaged (Project Pagination)
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
            p.Id,
            p.ProjectName,
            p.Description,
            p.StartDate,
            p.EndDate,
            (SELECT COUNT(*) FROM EmployeeProjects WHERE ProjectId = p.Id) AS AssignedEmployees,
            ROW_NUMBER() OVER (
                ORDER BY
                    CASE WHEN @SortBy = 'Id' AND @SortOrder = 'ASC' THEN p.Id END ASC,
                    CASE WHEN @SortBy = 'Id' AND @SortOrder = 'DESC' THEN p.Id END DESC,
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
        Id,
        ProjectName,
        Description,
        StartDate,
        EndDate,
        AssignedEmployees,
        (SELECT COUNT(*) FROM Projects p
         WHERE (@SearchTerm IS NULL 
            OR p.ProjectName LIKE '%' + @SearchTerm + '%'
            OR p.Description LIKE '%' + @SearchTerm + '%')) AS TotalCount
    FROM ProjectCTE
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO

PRINT 'All pagination stored procedures updated successfully!';
PRINT '';
PRINT 'Procedures created:';
PRINT '1. sp_GetEmployeesPaged - Employee pagination with department filter';
PRINT '2. sp_GetDepartmentsPaged - Department pagination';
PRINT '3. sp_GetProjectsPaged - Project pagination';
PRINT '';
PRINT 'Make sure to restart your .NET backend application.';
GO
