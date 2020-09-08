USE DataDB
GO
/****** Object:  StoredProcedure [dbo].[CALCULATE_SLOPE]    Script Date: 8/12/2020 11:42:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER   PROCEDURE [dbo].[CALCULATE_SLOPE]
	-- Add the parameters for the stored procedure here
	@LAYER_NAME_LINE  NVARCHAR(200),
	@OBJECTID  INT

AS
DECLARE @STATEMENT NVARCHAR(MAX)
DECLARE @OBJECTID_STR NVARCHAR(50)
SELECT @OBJECTID_STR = CAST(@OBJECTID AS NVARCHAR(50));

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @STATEMENT ='DECLARE @REC_COUNT INT;
		SELECT @REC_COUNT= COUNT(1)  FROM GisUser.'+@LAYER_NAME_LINE+' WHERE OBJECTID = '+@OBJECTID_STR +' AND  CHARINDEX(''_'',lf_Code_Manhol_Baladast)>0  AND   CHARINDEX(''_'',lf_Code_Manhol_Paindast)>0 ;
		IF @REC_COUNT = 0 BEGIN
		    PRINT(''returned'');
			RETURN;
		END; 
        DECLARE @LAYERNAME_POINT_START NVARCHAR(50);
		DECLARE @LAYERNAME_POINT_END NVARCHAR(50);

	    DECLARE @OBJECTID_STR_POINT_START NVARCHAR(50);
	    DECLARE @OBJECTID_STR_POINT_END NVARCHAR(50);
		DECLARE @STATE NVARCHAR(MAX)
		   
	       SET @LAYERNAME_POINT_START = (SELECT SUBSTRING(lf_Code_Manhol_Baladast,0,(len(lf_Code_Manhol_Baladast)+1 - CHARINDEX(''_'',reverse(lf_Code_Manhol_Baladast)))) FROM GisUser.'+@LAYER_NAME_LINE+' WHERE OBJECTID= '+@OBJECTID_STR+');
		   SET @OBJECTID_STR_POINT_START = (SELECT SUBSTRING(lf_Code_Manhol_Baladast,(len(lf_Code_Manhol_Baladast)+1 - CHARINDEX(''_'',reverse(lf_Code_Manhol_Baladast)))+1,LEN(lf_Code_Manhol_Baladast)) FROM GisUser.'+@LAYER_NAME_LINE+' WHERE OBJECTID= '+@OBJECTID_STR+');
		   SET @LAYERNAME_POINT_END = (SELECT SUBSTRING(lf_Code_Manhol_Paindast,0,(len(lf_Code_Manhol_Paindast)+1 - CHARINDEX(''_'',reverse(lf_Code_Manhol_Paindast)))) FROM GisUser.'+@LAYER_NAME_LINE+' WHERE OBJECTID= '+@OBJECTID_STR+');
		   SET @OBJECTID_STR_POINT_END = (SELECT SUBSTRING(lf_Code_Manhol_Paindast,(len(lf_Code_Manhol_Paindast)+1 - CHARINDEX(''_'',reverse(lf_Code_Manhol_Paindast)))+1,LEN(lf_Code_Manhol_Paindast)) FROM GisUser.'+@LAYER_NAME_LINE+' WHERE OBJECTID= '+@OBJECTID_STR+');

		
		   SET @STATE =''
		     	 DECLARE @SHAPE_LENGTH NUMERIC(38,8);
		         DECLARE @SLOPE_PARAMETER NUMERIC(38, 8);
		   	     DECLARE @HIEGHT_M  NUMERIC(38, 8);
	             DECLARE @Z_START NUMERIC(38, 8);
	             DECLARE @Z_END NUMERIC(38, 8);
				 DECLARE @DEPTH_START NUMERIC(38, 8);
		         DECLARE @DEPTH_END NUMERIC(38, 8);

				SET @DEPTH_START =( SELECT lf_Omgh_Ebteda FROM GisUser.'+@LAYER_NAME_LINE+' WHERE OBJECTID= '+@OBJECTID_STR+');
		        SET @DEPTH_END =(SELECT lf_Omgh_Enteha FROM GisUser.'+@LAYER_NAME_LINE+' WHERE OBJECTID= '+@OBJECTID_STR+');
		         SET @Z_START= (SELECT Z FROM GisUser.''+@LAYERNAME_POINT_START+'' WHERE OBJECTID = ''+@OBJECTID_STR_POINT_START+'');
		         SET @Z_END= (SELECT Z FROM GisUser.''+@LAYERNAME_POINT_END+'' WHERE OBJECTID = ''+@OBJECTID_STR_POINT_END+'');
	             SET  @HIEGHT_M= (@Z_START- @DEPTH_START) - (@Z_END-@DEPTH_END);
		         SET @SHAPE_LENGTH= (SELECT [Shape].STLength() FROM GisUser.['+ @LAYER_NAME_LINE+'] WHERE OBJECTID= '+@OBJECTID_STR+');
	             SET @SLOPE_PARAMETER= @HIEGHT_M / @SHAPE_LENGTH;
	             UPDATE GisUser.'+ @LAYER_NAME_LINE+' SET lf_Shib_Lule  = @SLOPE_PARAMETER*100 WHERE  OBJECTID = '+@OBJECTID_STR+';''
				 --PRINT @STATE
				 
		         EXEC (@STATE)
				 '
				 --PRINT @STATEMENT
		   EXEC SP_EXECUTESQL @STATEMENT
    -- Insert statements for procedure here
END

--exec  [dbo].[CALCULATE_SLOPE] 'PIPE_NN',7;