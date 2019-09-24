USE Diablo

--14
  SELECT TOP(50)
  	        [Name], FORMAT([Start], 'yyyy-MM-dd') AS [Start]
	    FROM Games
       WHERE DATEPART(YEAR, [Start]) IN (2011, 2012)
    ORDER BY [Start], Name

--15
SELECT * FROM (
				SELECT u.Username,
					   SUBSTRING(u.Email, CHARINDEX('@',u.Email) + 1, LEN(u.Email) - CHARINDEX('@', u.Email)) AS [Email Provider]
				  FROM Users AS u)
				    AS formatedTable
	 ORDER BY formatedTable.[Email Provider], formatedTable.Username

GO

--16
SELECT Username, IpAddress AS [IP Address]
  FROM Users
  WHERE IpAddress LIKE '___.1%.%.___'
  ORDER BY Username

GO

USE Diablo
--17
SELECT [Name] AS Game,
	[Part of the Day] = 
		CASE 
			WHEN DATEPART(HOUR, [Start]) < 12 THEN 'Morning'
			WHEN DATEPART(HOUR, [Start]) < 18 THEN 'Afternoon'
			ELSE 'Evening'
		END,
	Duration =
		CASE
			WHEN Duration <= 3 THEN 'Extra Short'
			WHEN Duration <= 6 THEN 'Short'
			WHEN Duration > 6 THEN 'Long'
			ELSE 'Extra Long'
		END
FROM Games
ORDER BY Game, Duration, [Part of the Day]

--SELECT [Name] AS Game,
--	CASE 
--		WHEN DATEPART(HOUR, [Start]) BETWEEN 0 AND 11 THEN 'Morning'
--		WHEN DATEPART(HOUR, [Start]) BETWEEN 12 AND 17 THEN 'Afternoon'
--		ELSE 'Evening'
--	END AS [Part of the Day],
--	CASE
--		WHEN Duration <= 3 THEN 'Extra Short'
--		WHEN Duration BETWEEN 4 AND 6 THEN 'Short'
--		WHEN Duration > 6 THEN 'Long'
--		ELSE 'Extra Long'
--	END AS Duration
--FROM Games
--ORDER BY Game, Duration, [Part of the Day]

--18
USE Orders

SELECT ProductName, 
	   OrderDate,
	   DATEADD(DAY, 3, OrderDate) AS [Pay Due],
	   DATEADD(MONTH, 1, OrderDate) AS [Deliver Due]
  FROM Orders

--19 not included in Judge !!!
CREATE TABLE People 
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL,
	Birthdate DATE NOT NULL
)

INSERT INTO People ([Name], Birthdate) VALUES
('Victor', '2000-12-07 00:00:00.000'),
('Steven', '1992-09-10 00:00:00.000'),
('Stephen', '1910-09-19 00:00:00.000'), 
('John', '2010-01-06 00:00:00.000')

SELECT [Name],
	DATEDIFF(YEAR, Birthdate, GETDATE()) AS [Age in Years],
	DATEDIFF(MONTH, Birthdate, GETDATE()) AS [Age in Months],
	DATEDIFF(DAY, Birthdate, GETDATE()) AS [Age in Dys],
	DATEDIFF(MINUTE, Birthdate, GETDATE()) AS [Age in Minutes]
  FROM People


