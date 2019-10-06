USE [Geography]

GO

--12. Highest Peaks in Bulgaria
  SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation 
    FROM Countries AS c
    JOIN MountainsCountries AS mc
      ON c.CountryCode = mc.CountryCode AND c.CountryCode = 'BG'
    JOIN Mountains AS m
      ON mc.MountainId = m.Id
    JOIN Peaks AS p
      ON p.MountainId = m.Id AND p.Elevation > 2835
ORDER BY p.Elevation DESC

--13. Count Mountain Ranges
SELECT c.CountryCode , COUNT(c.CountryCode) AS MountainRanges
  FROM Countries AS c
  JOIN MountainsCountries AS mc
    ON c.CountryCode = mc.CountryCode
  JOIN Mountains AS m
	ON mc.MountainId = m.Id 
  WHERE c.CountryCode IN ('BG', 'RU', 'US')
GROUP BY c.CountryCode

--14. Countries With or Without Rivers
SELECT TOP(5)
           c.CountryName, r.RiverName
      FROM Countries AS c
		   LEFT OUTER JOIN CountriesRivers AS cr
        ON cr.CountryCode = c.CountryCode
		   LEFT OUTER JOIN Rivers AS r
        ON r.Id = cr.RiverId
		   JOIN Continents as con
        ON con.ContinentCode = c.ContinentCode AND con.ContinentName = 'Africa'
  ORDER BY c.CountryName

--15. Continents and Currencies
  SELECT ranked.ContinentCode, ranked.CurrencyCode, ranked.CurrencyUsage 
    FROM 
 		 (SELECT gt.ContinentCode, gt.CurrencyCode, gt.CurrencyUsage,
 				 DENSE_RANK() OVER (PARTITION BY gt.ContinentCode ORDER BY gt.CurrencyUsage DESC) AS [Rank] 
 		    FROM 
 				 (SELECT ContinentCode, CurrencyCode , COUNT(CountryCode) AS CurrencyUsage
 				    FROM Countries
 			    GROUP BY ContinentCode, CurrencyCode) AS gt
 		   WHERE gt.CurrencyUsage > 1 ) AS ranked
   WHERE ranked.Rank = 1
ORDER BY ContinentCode

--16. Countries Without any Mountains
   SELECT 
     	  COUNT(c.CountryCode) AS Count
     FROM Countries AS c
LEFT JOIN MountainsCountries AS mc
	   ON mc.CountryCode = c.CountryCode
	WHERE mc.CountryCode IS NULL

--17. Highest Peak and Longest River by Country
SELECT TOP(5) 
		   c.CountryName, MAX(p.Elevation) AS HighestPiakElevation, MAX(r.[Length]) AS LongestRiverLength
      FROM Countries AS c
      JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
 LEFT JOIN Peaks AS p ON p.MountainId = mc.MountainId
 LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
 LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
  GROUP BY c.CountryName
  ORDER BY MAX(p.Elevation) DESC, MAX(r.[Length]) DESC

  --18. Highest Peak Name and Elevation by Country
   SELECT TOP(5)
  	      tt.CountryName AS Country, 
  	      IIF(tt.PeakName IS NULL, '(no highest peak)', tt.PeakName) AS [Highest Peak Name],
  	      IIF(tt.Elevation IS NULL, 0, tt.Elevation) AS [Highest Peak Elevation],
  	      IIF(tt.MountainRange IS NULL, '(no mountain)', tt.MountainRange) AS [Mountain]
    FROM
  	     (SELECT c.CountryName, p.PeakName, p.Elevation, m.MountainRange,
  	             DENSE_RANK() OVER (PARTITION BY c.CountryNAme ORDER BY p.Elevation DESC) AS [Rank]
  	       FROM Mountains AS m
  	       JOIN Peaks AS p ON p.MountainId = m.Id
  	       JOIN MountainsCountries AS mc ON mc.MountainId = m.Id
  	 RIGHT JOIN Countries AS c ON c.CountryCode = mc.CountryCode) AS tt
   WHERE tt.Rank = 1
ORDER BY tt.CountryName, tt.PeakName
     
	


