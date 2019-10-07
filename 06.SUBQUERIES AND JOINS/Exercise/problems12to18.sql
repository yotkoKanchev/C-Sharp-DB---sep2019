USE [Geography]

GO

--12. Highest Peaks in Bulgaria
  SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation
    FROM Countries AS c
    JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
    JOIN Mountains AS m ON m.Id = mc.MountainId
    JOIN Peaks AS p ON p.MountainId = m.Id
   WHERE c.CountryName = 'Bulgaria' AND p.Elevation > 2835
ORDER BY p.Elevation DESC

--13. Count Mountain Ranges
SELECT c.CountryCode, COUNT(mc.CountryCode) AS MountainRanges
  FROM Countries AS c
  JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
  WHERE c.CountryCode IN ('BG', 'US', 'RU')
GROUP BY c.CountryCode

--14. Countries With or Without Rivers
SELECT TOP(5) c.CountryName, r.RiverName 
      FROM Countries AS c
      LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
      LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
      JOIN Continents AS con ON con.ContinentCode = c.ContinentCode
     WHERE con.ContinentName ='Africa'
  ORDER BY c.CountryName

--15. Continents and Currencies
SELECT grouped.ContinentCode, grouped.CurrencyCode, grouped.CurrencyUsage 
  FROM  
	    (SELECT ContinentCode, CurrencyCode,
  	            COUNT(CountryCode) AS CurrencyUsage,
  	            DENSE_RANK() OVER (PARTITION BY ContinentCode ORDER BY COUNT(CountryCode) DESC) AS [Rank]
		   FROM Countries
       GROUP BY ContinentCode, CurrencyCode
         HAVING COUNT(CountryCode) > 1 ) AS grouped
 WHERE grouped.[Rank] = 1

--16. Countries Without any Mountains
   SELECT COUNT(c.CountryCode) AS [Count]
     FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
    WHERE mc.CountryCode IS NULL

--17. Highest Peak and Longest River by Country
	--a
SELECT TOP(5) f.CountryName, 
			  f.Elevation AS HighestPeakElevation, 
			  f.[Length] AS LongestRiverLength
      FROM 
           (SELECT c.CountryName, p.Elevation, r.[Length],
  	               DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY p.Elevation DESC) AS PeakRank,
  	               DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY r.[Length] DESC) AS RiverRank
  	          FROM Countries AS c
  	          JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
  	          JOIN Peaks AS p ON p.MountainId = mc.MountainId
  	          JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
  	          JOIN Rivers AS r ON r.Id = cr.RiverId) AS f
     WHERE f.PeakRank = 1 AND f.RiverRank = 1
  ORDER BY f.Elevation DESC, f.Length DESC

	--b
SELECT TOP(5) c.CountryName, MAX(p.Elevation) AS HighestPiakElevation, MAX(r.[Length]) AS LongestRiverLength
      FROM Countries AS c
      JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
 LEFT JOIN Peaks AS p ON p.MountainId = mc.MountainId
 LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
 LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
  GROUP BY c.CountryName
  ORDER BY MAX(p.Elevation) DESC, MAX(r.[Length]) DESC

--18. Highest Peak Name and Elevation by Country
SELECT TOP(5)
	       ranked.CountryName AS Country,
	       IIF(ranked.PeakName IS NULL, '(no highest peak)', ranked.PeakName) AS [Highest Peak Name],
	       IIF(ranked.Elevation IS NULL, 0, ranked.Elevation) AS [Highest Peak Elevation],
	       IIF(ranked.MountainRange IS NULL, '(no mountain)', ranked.MountainRange) AS Mountain 
      FROM 
           (SELECT c.CountryName, p.PeakName, p.Elevation, m.MountainRange,
           	   DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY p.Elevation DESC) AS [Rank]
             FROM Countries AS c
             LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
             LEFT JOIN Mountains AS m ON m.Id = mc.MountainId
             LEFT JOIN Peaks AS p ON p.MountainId = m.Id) AS ranked
     WHERE ranked.[Rank] = 1
  ORDER BY ranked.CountryName, ranked.PeakName

