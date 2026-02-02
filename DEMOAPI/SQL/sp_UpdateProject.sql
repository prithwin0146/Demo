CREATE OR ALTER PROCEDURE sp_UpdateProject
    @ProjectId INT,
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
        
        -- Check if project exists
        IF NOT EXISTS (SELECT 1 FROM Projects WHERE ProjectId = @ProjectId)
        BEGIN
            RAISERROR('Project with ID %d not found.', 16, 1, @ProjectId);
            RETURN;
        END
        
        -- Update the project
        UPDATE Projects
        SET 
            ProjectName = @ProjectName,
            Description = @Description,
            StartDate = @StartDate,
            EndDate = @EndDate,
            Status = @Status
        WHERE ProjectId = @ProjectId;
        
        -- Return the number of rows affected
        SELECT @@ROWCOUNT AS RowsAffected;
        
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
