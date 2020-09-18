create or alter   PROCEDURE [dbo].[PROC_CREATE_OBJECTID] 
   @SHAPE NVARCHAR(MAX),
   @SRID INT,
   @LAYER_NAME NVARCHAR(50),
   @FIELDS NVARCHAR(max),
   @VALUES NVARCHAR(50),
   @OBJECTID INT OUTPUT
AS
DECLARE @STATEMENT NVARCHAR(MAX);
DECLARE @ParmDefinition NVARCHAR(500);
BEGIN
/*
-- in case of database is geodatabase
EXEC DBO.NEXT_ROWID 'DBO', @LAYER_NAME, @OBJECTID OUTPUT;
SET @STATEMENT = 'INSERT INTO [' + @LAYER_NAME +'] (shape, OBJECTID '+@FIELDS+' ) VALUES(geometry::STGeomFromText(''' + @SHAPE +''',' + CONVERT(NVARCHAR(10),@SRID) + '),'+CONVERT(NVARCHAR(50),@OBJECTID)+''+@VALUES+''
EXEC (@STATEMENT)*/

--- in case of objectid is auto increment
SET @STATEMENT = 'declare @p1 table (oid int);INSERT INTO [' + @LAYER_NAME +'] (shape '+@FIELDS+' )  output INSERTED.OBJECTID into @p1 VALUES(geometry::STGeomFromText(''' + @SHAPE +''',' + CONVERT(NVARCHAR(10),@SRID) + ')'+@VALUES+');set  @OBJECTID_OUT = (select MAX(oid) from @p1) '
SET @ParmDefinition = N'@OBJECTID_OUT INT OUTPUT';  
EXEC  sp_executesql  @STATEMENT,@ParmDefinition,@OBJECTID_OUT=@OBJECTID OUTPUT;

END