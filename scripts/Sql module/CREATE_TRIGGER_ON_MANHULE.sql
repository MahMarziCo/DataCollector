USE [homepiniir_dc_dc_data]
GO
/****** Object:  Trigger [dbo].[TRG_MANHOLE_NN]    Script Date: 8/11/2020 11:33:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER   TRIGGER [dbo].[TRG_MANHOLE_NN]
   ON  [dbo].[MANHOLE_NN]
   AFTER  UPDATE,INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @OBJECTID INT;
	DECLARE MANHULE_CURSOR CURSOR FOR 
		SELECT OBJECTID FROM INSERTED 
         OPEN MANHULE_CURSOR
		 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		 WHILE @@FETCH_STATUS = 0  
		 BEGIN
		 --PRINT @OBJECTID;
		 EXEC [dbo].[PROC_ON_MANHULE]  'MANHOLE_NN',@OBJECTID;
	--exec  [dbo].[PROC_ON_MANHULE] 'MANHOLE_NN', 'PIPE_TOTAL'
    -- Insert statements for trigger here
	 FETCH NEXT FROM MANHULE_CURSOR INTO @OBJECTID;
		END;
		CLOSE MANHULE_CURSOR;  
        DEALLOCATE MANHULE_CURSOR;
END
