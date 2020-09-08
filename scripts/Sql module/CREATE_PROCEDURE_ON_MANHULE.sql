USE DataDBCopy
GO
/****** Object:  StoredProcedure [dbo].[PROC_ON_MANHULE]    Script Date: 8/12/2020 11:54:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create or ALTER   PROCEDURE  [dbo].[PROC_ON_MANHULE] 
	-- Add the parameters for the stored procedure here
    @LAYER_NAME NVARCHAR(50),
	@OBJECTID INT
AS
DECLARE @STATEMENT NVARCHAR(MAX)
BEGIN

	SET NOCOUNT ON;
SET @STATEMENT ='
	DECLARE @POINT_ID INT;
	DECLARE @SHAPE GEOMETRY;

	DECLARE @FAZELABRO NVARCHAR(50);
	DECLARE @TRAFIC NVARCHAR(50);
	DECLARE @DEPTH numeric(38, 8);
	DECLARE @PIPE_LAYER NVARCHAR(50);
	DECLARE @PIPE_OBJECTID INT;

		SELECT 
	    @POINT_ID= OBJECTID,
		@SHAPE =SHAPE,
		@DEPTH = mf_Omgh,
		@FAZELABRO = mf_Mahale_Gharargiri,
		@TRAFIC = mf_Shedat_Terafic FROM GisUser.'+@LAYER_NAME+' WHERE OBJECTID ='+ CONVERT(NVARCHAR(50),@OBJECTID)+';
		PRINT CONVERT(NVARCHAR(50),@SHAPE)
		DECLARE CREATE_CURSOR CURSOR FOR 
		SELECT OBJECTID,LAYER_NAME FROM PIPE_TOTAL WHERE SHAPE.STIntersects(@SHAPE.STBuffer(0.001))=1 
         OPEN CREATE_CURSOR
		 FETCH NEXT FROM CREATE_CURSOR INTO @PIPE_OBJECTID, @PIPE_LAYER;
		 WHILE @@FETCH_STATUS = 0  
         BEGIN 
		    DECLARE @STATEMENT NVARCHAR(MAX)
			DECLARE @DEPTH_STR NVARCHAR(50);
			DECLARE @POINTID_STR NVARCHAR(50);
			DECLARE @OBJECTID_STR NVARCHAR(50);
			DECLARE @SHAPE_STR NVARCHAR(MAX);

			SET @SHAPE_STR =@SHAPE.STAsText();
			SET @DEPTH_STR =CONVERT(NVARCHAR(50),@DEPTH);
			SET @POINTID_STR =CONVERT(NVARCHAR(50),@POINT_ID);
			SET @OBJECTID_STR =CONVERT(NVARCHAR(50),@PIPE_OBJECTID);
		    SET @STATEMENT =''
			DECLARE @POINT_SHAPE GEOMETRY
			SET @POINT_SHAPE =geometry::STGeomFromText(''''''+@SHAPE_STR+'''''',32639).STBuffer(0.001)

		    UPDATE GisUser.''+@PIPE_LAYER+'' SET lf_Code_Manhol_Baladast = '''''+@LAYER_NAME+'''''+''''_''''+ ''''''+@POINTID_STR+''''''  
		    WHERE SHAPE.STStartPoint().STIntersects(@POINT_SHAPE) =1 AND (lf_Code_Manhol_Baladast IS NULL  OR lf_Code_Manhol_Baladast ='''''''');

		    UPDATE GisUser.''+@PIPE_LAYER+'' SET lf_Code_Manhol_Paindast = '''''+@LAYER_NAME+'''''+''''_''''+ ''''''+@POINTID_STR+'''''' 
		    WHERE SHAPE.STEndPoint().STIntersects(@POINT_SHAPE) =1 AND (lf_Code_Manhol_Paindast IS NULL OR lf_Code_Manhol_Baladast ='''''''');

		    UPDATE GisUser.''+@PIPE_LAYER+'' SET lf_Omgh_Ebteda = ''+@DEPTH_STR+''
		    WHERE SHAPE.STStartPoint().STIntersects(@POINT_SHAPE) =1 AND lf_Omgh_Ebteda IS NULL;

		    UPDATE GisUser.''+@PIPE_LAYER+'' SET lf_Omgh_Enteha = ''+@DEPTH_STR+''
		    WHERE SHAPE.STEndPoint().STIntersects(@POINT_SHAPE) =1 AND lf_Omgh_Enteha IS NULL;

		    UPDATE GisUser.''+@PIPE_LAYER+'' SET lf_Mahale_Gharar_Fazelabro = ''''''+@FAZELABRO+'''''' 
		    WHERE SHAPE.STStartPoint().STIntersects(@POINT_SHAPE) =1 AND (lf_Mahale_Gharar_Fazelabro IS NULL OR lf_Mahale_Gharar_Fazelabro ='''''''');

	     	UPDATE GisUser.''+@PIPE_LAYER+'' SET lf_Shedate_BareTerafik = ''''''+@TRAFIC+'''''' 
		    WHERE SHAPE.STStartPoint().STIntersects(@POINT_SHAPE) =1 AND (lf_Shedate_BareTerafik IS NULL OR lf_Shedate_BareTerafik ='''''''');

			EXEC [dbo].[CALCULATE_SLOPE] ''''''+@PIPE_LAYER+'''''', ''+@OBJECTID_STR+''''
			--PRINT @STATEMENT
            EXEC SP_EXECUTESQL @STATEMENT	

        FETCH NEXT FROM CREATE_CURSOR INTO @PIPE_OBJECTID, @PIPE_LAYER;
		END;
		CLOSE CREATE_CURSOR;  
        DEALLOCATE CREATE_CURSOR'

--PRINT @STATEMENT
EXEC SP_EXECUTESQL @STATEMENT	
END

-- exec  [dbo].[PROC_ON_MANHULE] 'Manhole', 460

select * from GisUser.MANHOLE