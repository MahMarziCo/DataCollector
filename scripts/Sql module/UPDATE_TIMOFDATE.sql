SELECT 
'UPDATE [GISUSER].'+CLASS_NAME+  
' SET '+DATEOF+'= SUBSTRING('+DATEOF+',1,CHARINDEX('' '', '+DATEOF+')),'
+TIMEOF+' = SUBSTRING('+DATEOF+',CHARINDEX('' '', '+DATEOF+')+1,LEN('+DATEOF+'))'
+' WHERE CHARINDEX('' '', '+DATEOF+') >0 and len('+DATEOF+')>=12'
 FROM [TB_CLASSES] 
 WHERE DATEOF IS NOT NULL AND TIMEOF IS NOT NULL