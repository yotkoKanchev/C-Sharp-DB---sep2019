USE Geography

SELECT PeakName FROM Peaks
ORDER BY PeakName

GO

SELECT TOP(30) CountryName, [Population] FROM Countries
WHERE ContinentCode = 'EU'
ORDER BY [Population] DESC

GO

SELECT 
CountryName, 
CountryCode,
CASE
    WHEN CurrencyCode != 'EUR' OR CurrencyCode IS NULL THEN 'Not Euro'
    ELSE 'Euro'
END AS Currency
FROM Countries
ORDER BY CountryName 
