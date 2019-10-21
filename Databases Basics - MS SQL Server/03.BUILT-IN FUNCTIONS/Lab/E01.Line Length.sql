SELECT Id, 
	   SQRT(SQUARE(X1-X2) + SQUARE(Y1-Y2)) 
    AS Legth
  FROM Lines