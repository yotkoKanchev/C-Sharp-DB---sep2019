SELECT 
	CEILING (
		CEILING (
			CAST(Quantity AS FLOAT) / BoxCapacity
			    ) / 
		    PalletCapacity
		    ) 
    AS [Number of pallets]	
  FROM Products





