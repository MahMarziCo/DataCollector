SELECT 
'UPDATE [homepiniir_dc_dc_logic].[DBO].[TB_CLASSES] SET TIMEOF= '''+DATEOF+'_TIME'' WHERE Class_name = '''+CLASS_NAME+''';',
'UPDATE [homepiniir_dc_dc_data].[DBO].'+CLASS_NAME+' SET '+TIMEOF+' =SUBSTRING('+DATEOF+',12,5);
UPDATE [homepiniir_dc_dc_data].[DBO].'+CLASS_NAME+' SET '+DATEOF+'=SUBSTRING('+DATEOF+',0,12);'

 FROM [homepiniir_dc_dc_logic].[DBO].[TB_CLASSES]
 WHERE DATEOF IS NOT NULL AND TIMEOF IS NOT NULL