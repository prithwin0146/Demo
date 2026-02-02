CREATE OR ALTER PROCEDURE sp_CreateProject
    @ProjectName NVARCHAR(255),
    @Description NVARCHAR(MAX),
    @StartDate DATETIME,
    @EndDate DATETIME = NULL,
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Insert the new project
        INSERT INTO Projects (ProjectName, Description, StartDate, EndDate, Status)
        VALUES (@ProjectName, @Description, @StartDate, @EndDate, @Status);
        
        -- Return the newly created ProjectId
        SELECT SCOPE_IDENTITY() AS ProjectId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Rethrow the error
        THROW;
    END CATCH
END;
GO
