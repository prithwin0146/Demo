-- =============================================
-- Department Pagination with CTE
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
                    CASE WHEN @SortBy = 'DepartmentName' AND @SortOrder = 'DESC' THEN d.DepartmentName END DESC,
                    CASE WHEN @SortBy = 'EmployeeCount' AND @SortOrder = 'ASC' 
                         THEN (SELECT COUNT(*) FROM Employees WHERE DepartmentId = d.DepartmentId) END ASC,
                    CASE WHEN @SortBy = 'EmployeeCount' AND @SortOrder = 'DESC' 
                         THEN (SELECT COUNT(*) FROM Employees WHERE DepartmentId = d.DepartmentId) END DESC
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
        (SELECT COUNT(*) FROM DepartmentCTE) AS TotalCount
    FROM DepartmentCTE
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO