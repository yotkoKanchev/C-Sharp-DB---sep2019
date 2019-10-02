USE Geography

--a
  SELECT m.MountainRange, p.PeakName, p.Elevation 
    FROM Peaks AS p , Mountains AS m
   WHERE p.MountainId = 
						(SELECT Id
						   FROM Mountains 
						  WHERE MountainRange = 'Rila') 
						    AND m.MountainRange = 'Rila'
ORDER BY p.Elevation DESC

--b

SELECT m.MountainRange, p.PeakName, p.Elevation 
    FROM Mountains AS m
    JOIN Peaks AS p ON p.MountainId = m.Id
   WHERE m.MountainRange = 'Rila'
ORDER BY p.Elevation DESC


  