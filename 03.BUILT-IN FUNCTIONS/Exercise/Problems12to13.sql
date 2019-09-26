USE [Geography]

--12. Countries Holding 'A'
	--a
  SELECT CountryName AS [Country Name], IsoCode AS [ISO Code] 
    FROM Countries
   WHERE CountryName LIKE '%a%a%a%'
ORDER BY IsoCode

	--b
  SELECT CountryName AS [Country Name], IsoCode AS [ISO Code] 
    FROM Countries
   WHERE LEN(CountryName) - LEN(REPLACE(CountryName, 'a', '')) >= 3
ORDER BY IsoCode

--13. Mix of Peak and River Names	
	--a
  SELECT p.PeakName, r.RiverName,
         LOWER( CONCAT( LEFT( p.PeakName, LEN(p.PeakName ) -1 ), r.RiverName )) AS Mix
    FROM Peaks AS p, Rivers AS r
   WHERE RIGHT(p.PeakName, 1) = LEFT(r.RiverName, 1)
ORDER BY Mix
	--b
  SELECT p.PeakName, r.RiverName, LOWER(PeakName + SUBSTRING(RiverName, 2, LEN(RiverName) - 1)) AS Mix 
    FROM Peaks AS p
    JOIN Rivers AS r
      ON RIGHT(PeakName, 1) = LEFT(RiverName, 1)
ORDER BY Mix