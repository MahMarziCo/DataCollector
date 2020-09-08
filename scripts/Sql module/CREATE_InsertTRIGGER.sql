USE [DataDB]
GO
/****** Object:  StoredProcedure [GisUser].[InsertTRIGGER]    Script Date: 8/2/2020 10:21:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [GisUser].[InsertTRIGGER]
AS 
BEGIN

DECLARE 
    @sql nvarchar(max), 
    @statement nvarchar(max)

   -- exec sp_executesql N'DROP TRIGGER  GisUser.TRG_INSERT_PIPE';
   -- exec sp_executesql N'DROP TRIGGER  GisUser.TRG_INSERT_PIPE_M5';
   -- exec sp_executesql N'DROP TRIGGER  GisUser.TRG_INSERT_PIPE_M3';
   -- exec sp_executesql N'DROP TRIGGER  GisUser.TRG_MANHOLE_UPDATE';
   -- exec sp_executesql N'DROP TRIGGER  GisUser.TRG_MANHOL_UPDATE';
   -- exec sp_executesql N'DROP TRIGGER  GisUser.TRG_MANHOLE_M5_UPDATE';
   -- exec sp_executesql N'DROP TRIGGER  GisUser.TRG_MANHOLE_M3_UPDATE';

IF OBJECT_ID(N'GisUser.TRG_INSERT_PIPE', N'TR') IS  NULL
begin
SET @statement =' create TRIGGER [GisUser].TRG_INSERT_PIPE
   ON  DATADB.GISUSER.PIPE
   AFTER INSERT
AS 
BEGIN
SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE PIPE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN PIPE_CURSOR
		 FETCH NEXT FROM PIPE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 EXEC [dbo].[PROC_ON_PIPELINE]  ''PIPE'',@OBJECTID;
	 FETCH NEXT FROM PIPE_CURSOR INTO @OBJECTID;
		END;
		CLOSE PIPE_CURSOR;  
        DEALLOCATE PIPE_CURSOR;	

END;';

 SET @sql = 
    'EXEC ' + QUOTENAME('datadb') + '.sys.sp_executesql 
                  N''EXEC(@statement)''
                , N''@statement nvarchar(max)''
                , @statement;'

EXEC sp_executeSQL @sql, N'@statement nvarchar(max)', @statement
end;


IF OBJECT_ID(N'GisUser.TRG_INSERT_PIPE', N'TR') IS  NULL
begin
SET @statement =' create TRIGGER [GisUser].TRG_INSERT_PIPE
   ON  DATADB.GISUSER.PIPE_M5
   AFTER INSERT
AS 
BEGIN
SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE PIPE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN PIPE_CURSOR
		 FETCH NEXT FROM PIPE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 EXEC [dbo].[PROC_ON_PIPELINE]  ''PIPE_M5'',@OBJECTID;
	 FETCH NEXT FROM PIPE_CURSOR INTO @OBJECTID;
		END;
		CLOSE PIPE_CURSOR;  
        DEALLOCATE PIPE_CURSOR;	

END;';

 SET @sql = 
    'EXEC ' + QUOTENAME('datadb') + '.sys.sp_executesql 
                  N''EXEC(@statement)''
                , N''@statement nvarchar(max)''
                , @statement;'

EXEC sp_executeSQL @sql, N'@statement nvarchar(max)', @statement
end;


IF OBJECT_ID(N'GisUser.TRG_INSERT_PIPE', N'TR') IS  NULL
begin
SET @statement =' create TRIGGER [GisUser].TRG_INSERT_PIPE
   ON  DATADB.GISUSER.PIPE_M3
   AFTER INSERT
AS 
BEGIN
SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE PIPE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN PIPE_CURSOR
		 FETCH NEXT FROM PIPE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 EXEC [dbo].[PROC_ON_PIPELINE]  ''PIPE_M3'',@OBJECTID;
	 FETCH NEXT FROM PIPE_CURSOR INTO @OBJECTID;
		END;
		CLOSE PIPE_CURSOR;  
        DEALLOCATE PIPE_CURSOR;	

END;';

 SET @sql = 
    'EXEC ' + QUOTENAME('datadb') + '.sys.sp_executesql 
                  N''EXEC(@statement)''
                , N''@statement nvarchar(max)''
                , @statement;'

EXEC sp_executeSQL @sql, N'@statement nvarchar(max)', @statement
end;



IF OBJECT_ID(N'GisUser.TRG_MANHOLE_UPDATE', N'TR') IS NULL
begin
SET @statement ='CREATE TRIGGER GisUser.TRG_MANHOLE_UPDATE
   ON  DATADB.GISUSER.MANHOLE
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE MANHULE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN MANHULE_CURSOR
		 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 EXEC [dbo].[PROC_ON_MANHULE]  ''MANHOLE'',@OBJECTID;

	 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		END;
		CLOSE MANHULE_CURSOR;  
        DEALLOCATE MANHULE_CURSOR;	

END;';

 SET @sql = 
    'EXEC ' + QUOTENAME('datadb') + '.sys.sp_executesql 
                  N''EXEC(@statement)''
                , N''@statement nvarchar(max)''
                , @statement;'

EXEC sp_executeSQL @sql, N'@statement nvarchar(max)', @statement
end;


IF OBJECT_ID(N'GisUser.TRG_MANHOL_UPDATE', N'TR') IS NULL
begin
SET @statement ='CREATE TRIGGER TRG_MANHOL_UPDATE
   ON  DATADB.GISUSER.MANHOL
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE MANHULE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN MANHULE_CURSOR
		 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 EXEC [dbo].[PROC_ON_MANHULE]  ''MANHOL'',@OBJECTID;

	 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		END;
		CLOSE MANHULE_CURSOR;  
        DEALLOCATE MANHULE_CURSOR;		

END;'

 SET @sql = 
    'EXEC ' + QUOTENAME('datadb') + '.sys.sp_executesql 
                  N''EXEC(@statement)''
                , N''@statement nvarchar(max)''
                , @statement;'

EXEC sp_executeSQL @sql, N'@statement nvarchar(max)', @statement
end;

	
	
	
	IF OBJECT_ID(N'GisUser.TRG_MANHOL_UPDATE', N'TR') IS NULL
begin
SET @statement ='CREATE TRIGGER TRG_MANHOL_UPDATE
   ON  DATADB.GISUSER.MANHOLE_M5
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE MANHULE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN MANHULE_CURSOR
		 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 EXEC [dbo].[PROC_ON_MANHULE]  ''MANHOLE_M5'',@OBJECTID;

	 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		END;
		CLOSE MANHULE_CURSOR;  
        DEALLOCATE MANHULE_CURSOR;		

END;'

 SET @sql = 
    'EXEC ' + QUOTENAME('datadb') + '.sys.sp_executesql 
                  N''EXEC(@statement)''
                , N''@statement nvarchar(max)''
                , @statement;'

EXEC sp_executeSQL @sql, N'@statement nvarchar(max)', @statement
end;

	
	
	
	IF OBJECT_ID(N'GisUser.TRG_MANHOL_UPDATE', N'TR') IS NULL
begin
SET @statement ='CREATE TRIGGER TRG_MANHOL_UPDATE
   ON  DATADB.GISUSER.MANHOLE_M3
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE MANHULE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN MANHULE_CURSOR
		 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 EXEC [dbo].[PROC_ON_MANHULE]  ''MANHOLE_M3'',@OBJECTID;

	 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		END;
		CLOSE MANHULE_CURSOR;  
        DEALLOCATE MANHULE_CURSOR;		

END;'

 SET @sql = 
    'EXEC ' + QUOTENAME('datadb') + '.sys.sp_executesql 
                  N''EXEC(@statement)''
                , N''@statement nvarchar(max)''
                , @statement;'

EXEC sp_executeSQL @sql, N'@statement nvarchar(max)', @statement
end;
	end;