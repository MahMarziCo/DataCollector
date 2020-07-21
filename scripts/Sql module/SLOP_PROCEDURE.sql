-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE or ALTER PROCEDURE  CREATE_TRIGGER_SLOPE
	-- Add the parameters for the stored procedure here
	@LAYER_NAME  NVARCHAR(200),
	@SLOPE_FIELD  NVARCHAR(200),
	@START_DEPTH_FIELD  NVARCHAR(200),
	@END_DEPTH_FIELD  NVARCHAR(200)

AS
DECLARE @STATEMENT NVARCHAR(MAX)
DECLARE @TRG_NAME NVARCHAR(200);
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @TRG_NAME= 'MAH_'+ @LAYER_NAME +'_SLOPE_TRG';
	IF OBJECT_ID(@TRG_NAME,N'TR') IS NOT NULL
	BEGIN
		SET @STATEMENT= 'DROP TRIGGER '+@TRG_NAME;
	EXEC SP_EXECUTESQL @STATEMENT;
	END;
	SET @STATEMENT =
	'CREATE TRIGGER '+@TRG_NAME+' ON '+@LAYER_NAME+' AFTER UPDATE,INSERT '+ 
	'AS BEGIN 
		DECLARE @OBJECTID INT
		DECLARE @DEPTH_START NUMERIC(38, 8)
		DECLARE @DEPTH_END NUMERIC(38, 8)
		DECLARE @SHAPE_LENGTH NUMERIC(38,8)
		DECLARE @SLOPE_PARAMETER NUMERIC(38, 8)

		DECLARE INSERTED_CURSOR CURSOR FOR  
    SELECT OBJECTID,[Shape].STLength(),['+@START_DEPTH_FIELD+'],['+@END_DEPTH_FIELD+'] FROM INSERTED I 
	WHERE (['+@START_DEPTH_FIELD+'] IS NOT NULL AND ['+@END_DEPTH_FIELD+'] IS NOT NULL)
		
		OPEN INSERTED_CURSOR;  

      FETCH NEXT FROM INSERTED_CURSOR
      INTO @OBJECTID,@SHAPE_LENGTH, @DEPTH_START, @DEPTH_END;   
 
      WHILE @@FETCH_STATUS = 0  
      BEGIN 
	   DECLARE @HIEGHT_M  NUMERIC(38, 8);
	   SET  @HIEGHT_M= @DEPTH_START - @DEPTH_END;
	   SET @SLOPE_PARAMETER= @HIEGHT_M / @SHAPE_LENGTH;
	   UPDATE '+ @LAYER_NAME+' SET ['+@SLOPE_FIELD+'] = @SLOPE_PARAMETER WHERE  OBJECTID =@OBJECTID;
	   FETCH NEXT FROM INSERTED_CURSOR
        INTO @OBJECTID,@SHAPE_LENGTH, @DEPTH_START, @DEPTH_END;    
        END 
		CLOSE INSERTED_CURSOR;  
        DEALLOCATE INSERTED_CURSOR   
        END;
            '
			EXEC SP_EXECUTESQL @STATEMENT
    -- Insert statements for procedure here

END
GO

 EXEC	[dbo].[CREATE_TRIGGER_SLOPE] 'PIPE_NN','Slope_Line','lf_Omgh_Ebteda','lf_Omgh_Enteha'