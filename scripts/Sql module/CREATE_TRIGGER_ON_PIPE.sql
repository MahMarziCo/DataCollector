USE [homepiniir_dc_dc_data]
GO
/****** Object:  Trigger [dbo].[TRG_PIPE_NN]    Script Date: 8/11/2020 11:36:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER   TRIGGER [dbo].[TRG_PIPE_NN]
   ON  [dbo].[PIPE_NN]
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
		 --PRINT @OBJECTID;
		 EXEC [dbo].[PROC_ON_PIPELINE]  'PIPE_NN',@OBJECTID;
	--exec  [dbo].[PROC_ON_MANHULE] 'MANHOLE_NN', 'PIPE_TOTAL'
    -- Insert statements for trigger here
	 FETCH NEXT FROM PIPE_CURSOR INTO @OBJECTID;
		END;
		CLOSE PIPE_CURSOR;  
        DEALLOCATE PIPE_CURSOR;
END
