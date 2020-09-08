USE DataDBCopy
GO
/****** Object:  StoredProcedure [dbo].[PROC_ON_PIPELINE]    Script Date: 8/13/2020 12:03:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create or ALTER  PROCEDURE [dbo].[PROC_ON_PIPELINE] 
	@LAYER_NAME NVARCHAR(50) ,
	@OBJECTID INT
AS
DECLARE @STATEMENT NVARCHAR(MAX)
BEGIN

	SET NOCOUNT ON;
 SET @STATEMENT ='
	DECLARE @PIPE_ID INT;
	DECLARE @FIRST_POINT GEOMETRY;
	DECLARE @LAST_POINT GEOMETRY;

	DECLARE @FIRST_POINT_ID INT;
	DECLARE @FIRST_POINT_LAYER VARCHAR(100);
	DECLARE @FAZELABRO NVARCHAR(50);
	DECLARE @TRAFIC NVARCHAR(50);
	DECLARE @FIRST_POINT_DEPTH numeric(38, 8);

	DECLARE @LAST_POINT_ID INT;
	DECLARE @LAST_POINT_LAYER NVARCHAR(100);
	DECLARE @LAST_POINT_DEPTH numeric(38, 8);
    SELECT 
	   @PIPE_ID= OBJECTID,
	    @FIRST_POINT =SHAPE.STStartPoint().STBuffer(0.001) ,
		@LAST_POINT =  SHAPE.STEndPoint().STBuffer(0.001) FROM GisUser.'+@LAYER_NAME+' WHERE OBJECTID ='+CONVERT(nvarchar(50),@OBJECTID)+';
	   

		SELECT TOP 1 @FIRST_POINT_ID = OBJECTID ,
					@FIRST_POINT_DEPTH = mf_Omgh ,
					@FIRST_POINT_LAYER = LAYER_NAME,
					@FAZELABRO = mf_Mahale_Gharargiri,
					@TRAFIC = mf_Shedat_Terafic
					FROM 
					(SELECT OBJECTID,LAYER_NAME,SHAPE,mf_Omgh,mf_Mahale_Gharargiri,mf_Shedat_Terafic FROM MANHULE_TOTAL WHERE SHAPE.STIntersects(@FIRST_POINT)=1) AS MANHULE
				
				    

		
		SELECT TOP 1 @LAST_POINT_ID = OBJECTID ,
					@LAST_POINT_DEPTH = mf_Omgh ,
					@LAST_POINT_LAYER = LAYER_NAME
					FROM 
					(SELECT OBJECTID,LAYER_NAME,SHAPE,mf_Omgh,mf_Mahale_Gharargiri,mf_Shedat_Terafic FROM MANHULE_TOTAL WHERE SHAPE.STIntersects(@LAST_POINT)=1) AS MANHULE
	   
	   UPDATE GisUser.'+@LAYER_NAME+' SET Code = @PIPE_ID,
	   lf_Code_Manhol_Baladast = @FIRST_POINT_LAYER +''_''+ CAST(@FIRST_POINT_ID AS VARCHAR),
	   lf_Code_Manhol_Paindast = @LAST_POINT_LAYER +''_''+ CAST(@LAST_POINT_ID AS VARCHAR),
	   lf_Omgh_Ebteda= @FIRST_POINT_DEPTH,
	   lf_Omgh_Enteha= @LAST_POINT_DEPTH,
	   lf_Mahale_Gharar_Fazelabro = @FAZELABRO,
	   lf_Shedate_BareTerafik = @TRAFIC
	   WHERE OBJECTID = '+CONVERT(nvarchar(50),@OBJECTID)+';
	   EXEC CALCULATE_SLOPE '''+@LAYER_NAME+''','+CONVERT(nvarchar(50),@OBJECTID)+'
	   '
	   PRINT @STATEMENT
	   EXEC (@STATEMENT)
END

--exec [dbo].[PROC_ON_PIPELINE]  'pipe',100
--select OBJECTID,lf_Shib_Lule,lf_Code_Manhol_Baladast,lf_Code_Manhol_Paindast from GisUser.pipe
--update GisUser.pipe set lf_Code_Manhol_Paindast='',lf_Code_Manhol_Baladast='' where OBJECTID =100