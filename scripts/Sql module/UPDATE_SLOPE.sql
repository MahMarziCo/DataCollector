		UPDATE P  SET lf_shib_lule =(((MS.Z-P.lf_Omgh_Ebteda) - (ME.Z-P.lf_Omgh_Enteha))/P.[Shape].STLength() )
		FROM GisUser.PIPE AS P
		LEFT JOIN MANHULE_TOTAL AS MS ON P.SHAPE.STStartPoint().STBuffer(0.001).STIntersects(MS.SHAPE)=1
		LEFT JOIN MANHULE_TOTAL AS ME ON P.SHAPE.STEndPoint().STBuffer(0.001).STIntersects(ME.SHAPE)=1 
		WHERE  lf_Omgh_Ebteda IS NOT NULL AND lf_Omgh_Enteha IS NOT NULL