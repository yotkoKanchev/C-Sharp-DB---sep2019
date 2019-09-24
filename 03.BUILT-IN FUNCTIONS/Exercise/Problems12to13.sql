USE [Geography]

--12
  SELECT CountryName, IsoCode
    FROM Countries
   WHERE CountryName LIKE '%a%a%a%'
ORDER BY IsoCode

SELECT * FROM Countries

--13
--  SELECT p.PeakName, r.RiverName, LOWER(PeakName + SUBSTRING(RiverName, 2, LEN(RiverName) - 1)) AS Mix 
--    FROM Peaks AS p
--    JOIN Rivers AS r
--      ON RIGHT(PeakName, 1) = LEFT(RiverName, 1)
--ORDER BY Mix

  SELECT p.PeakName, r.RiverName, LOWER(CONCAT(LEFT(p.PeakName, LEN(p.PeakName) -1), r.RiverName)) AS Mix
    FROM Peaks AS p, Rivers AS r
   WHERE RIGHT(p.PeakName, 1) = LEFT(r.RiverName, 1)
ORDER BY Mix