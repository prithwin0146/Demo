CREATE OR ALTER PROCEDURE sp_GetEmployeesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortBy NVARCHAR(50) = 'Id',
    @SortOrder NVARCHAR(4) = 'ASC',
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate sort column to prevent SQL injection
    IF @SortBy NOT IN ('Id', 'Name', 'Email', 'JobRole')
        SET @SortBy = 'Id';

    IF @SortOrder NOT IN ('ASC', 'DESC')
        SET @SortOrder = 'ASC';

    -- Calculate offset
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Build dynamic SQL for sorting
    DECLARE @SQL NVARCHAR(MAX);

    SET @SQL = N'
    WITH EmployeeCTE AS (
        SELECT 
            e.Id,
            e.Name,
            e.Email,
            e.JobRole,
            e.Role,
            COUNT(*) OVER() AS TotalCount
        FROM Employees e
        WHERE (@SearchTerm IS NULL 
            OR e.Name LIKE ''%'' + @SearchTerm + ''%''
            OR e.Email LIKE ''%'' + @SearchTerm + ''%''
            OR e.JobRole LIKE ''%'' + @SearchTerm + ''%'')
    )
    SELECT 
        Id,
        Name,
        Email,
        JobRole,
        Role,
        TotalCount
    FROM EmployeeCTE
    ORDER BY ' + QUOTENAME(@SortBy) + ' ' + @SortOrder + '
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;';

    -- Execute dynamic SQL
    EXEC sp_executesql @SQL,
        N'@SearchTerm NVARCHAR(100), @Offset INT, @PageSize INT',
        @SearchTerm, @Offset, @PageSize;
END
GO  