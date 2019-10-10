USE Bank

GO

--09. Find Full Name
CREATE PROC usp_GetHoldersFullName 
AS
	SELECT CONCAT(FirstName, ' ', LastName) AS [Full Name]
      FROM AccountHolders

EXEC dbo.usp_GetHoldersFullName

GO

--10. People with Balance Higher Than
CREATE PROC usp_GetHoldersWithBalanceHigherThan @amount FLOAT
AS
	SELECT ah.FirstName AS [First Name], ah.LastName AS [Last Name] 
	  FROM AccountHolders AS ah
	  JOIN Accounts AS a ON a.AccountHolderId = ah.Id
	  GROUP BY ah.FirstName, ah.LastName
	  HAVING SUM(a.Balance) > @amount
	  ORDER BY ah.FirstName, ah.LastName

EXEC dbo.usp_GetHoldersWithBalanceHigherThan 35000.00

GO

--11. Future Value Function
CREATE FUNCTION ufn_CalculateFutureValue (@sum DECIMAL(20,2), @interestRate FLOAT, @years INT)
RETURNS DECIMAL(18,4)
AS
BEGIN
	RETURN @sum * POWER(1 + @interestRate, @years)
END

GO

SELECT dbo.ufn_CalculateFutureValue(100000, 0.005, 10)

GO

--12. Calculating Interest
CREATE PROC usp_CalculateFutureValueForAccount @accountId INT, @interestRate FLOAT
AS
	SELECT ah.Id AS [Account Id],
		   ah.FirstName AS [First Name],
		   ah.LastName AS [Last Name],
		   a.Balance AS [Current Balance],
		   dbo.ufn_CalculateFutureValue(a.Balance, @interestRate, 5) AS [Balance in 5 years]
	  FROM AccountHolders AS ah
	  JOIN Accounts AS a ON a.AccountHolderId = ah.Id
	 WHERE a.Id = @accountId 

EXEC dbo.usp_CalculateFutureValueForAccount 1, 0.1

GO

USE Diablo

GO

--13. *Cash in User Games Odd Rows
CREATE FUNCTION ufn_CashInUsersGames (@gameName VARCHAR(max))
RETURNS TABLE
AS
RETURN 
	SELECT SUM(ranked.Cash) AS [SumCash]
	  FROM (SELECT ug.Cash, ROW_NUMBER() OVER (ORDER BY ug.Cash DESC) AS [Rank]
	          FROM Games AS g
	          JOIN UsersGames AS ug ON ug.GameId = g.Id
	         WHERE g.[Name] = @gameName) AS ranked
	 WHERE ranked.[Rank] % 2 <> 0

GO

SELECT * FROM dbo.ufn_CashInUsersGames ('Love in a mist')