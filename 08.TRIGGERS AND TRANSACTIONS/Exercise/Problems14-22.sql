USE Bank

GO

--14. Create Table Logs
CREATE TABLE Logs
			 (
				LogId INT PRIMARY KEY IDENTITY,
				AccountId INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL,
				OldSum DECIMAL(20,2) NOT NULL,
				NewSum DECIMAL(20,2) NOT NULL
			 )

GO

CREATE TRIGGER tr_AccountsUpdate ON Accounts FOR UPDATE
AS
	INSERT INTO Logs
		 SELECT i.Id, d.Balance, i.Balance 
		   FROM inserted AS i
		   JOIN deleted AS d ON d.Id = i.Id

GO

UPDATE Accounts
   SET Balance -= 10
 WHERE Id = 1

GO

--15. Create Table Emails
CREATE TABLE NotificationEmails
			(
				Id INT PRIMARY KEY IDENTITY,
				Recipient INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL ,
				[Subject] VARCHAR(100) NOT NULL,
				Body VARCHAR(100)
			)

GO

CREATE TRIGGER tr_LogsInsert ON Logs FOR INSERT
AS
	INSERT INTO NotificationEmails
		 SELECT i.AccountId, 
				CONCAT('Balance change for account: ', i.AccountId), 
				CONCAT('On ', GETDATE(), ' your balance was changed from ' ,i.OldSum,' to ', i.NewSum, '.')
		   FROM inserted AS i

GO

UPDATE Accounts
   SET Balance -= 10
 WHERE Id IN (1,2,3,4,5)

SELECT * FROM NotificationEmails
SELECT * FROM Logs

GO

--16. Deposit Money
CREATE PROC usp_DepositMoney (@AccountId INT, @MoneyAmount MONEY) 
AS
BEGIN TRANSACTION
	IF (@MoneyAmount > 0)
	BEGIN
		UPDATE Accounts
		SET Balance += @MoneyAmount
		WHERE Id = @AccountId

		IF @@ROWCOUNT != 1
		BEGIN
			ROLLBACK
			RAISERROR('Invalid account!', 16, 1)
			RETURN
		END
	END
COMMIT

GO

EXEC dbo.usp_DepositMoney 1, 10

GO 

--17. Withdraw Money Procedure
CREATE PROC usp_WithdrawMoney @accountId INT, @moneyAmount MONEY
AS
BEGIN TRANSACTION
	IF (@MoneyAmount > 0)
	BEGIN
		UPDATE Accounts
		SET Balance -= @MoneyAmount
		WHERE Id = @AccountId

		IF @@ROWCOUNT != 1
		BEGIN
			ROLLBACK
			RAISERROR('Invalid account!', 16, 1)
			RETURN
		END
	END
COMMIT

GO

EXEC dbo.usp_WithdrawMoney 5, 25

GO

--18. Money Transfer
CREATE PROC usp_TransferMoney @senderId INT, @receiverId INT, @amount MONEY 
AS
	EXEC dbo.usp_DepositMoney @receiverId, @amount
	EXEC dbo.usp_WithdrawMoney @senderId, @amount

GO 

EXEC dbo.usp_TransferMoney 5, 1 , 5000

GO

USE Diablo
--19. Trigger
CREATE TRIGGER tr_UserGameItems ON UserGameItems INSTEAD OF INSERT AS
BEGIN 
	INSERT INTO UserGameItems
	SELECT i.Id, ug.Id FROM inserted
	JOIN UsersGames AS ug
	ON UserGameId = ug.Id
	JOIN Items AS i
	ON ItemId = i.Id
	WHERE ug.Level >= i.MinLevel
END
GO

UPDATE UsersGames
SET Cash += 50000
FROM UsersGames AS ug
JOIN Users AS u
ON ug.UserId = u.Id
JOIN Games AS g
ON ug.GameId = g.Id
WHERE g.Name = 'Bali' AND u.Username IN('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
GO

CREATE PROC usp_BuyItems(@Username VARCHAR(100)) AS
BEGIN
	DECLARE @UserId INT = (SELECT Id FROM Users WHERE Username = @Username)
	DECLARE @GameId INT = (SELECT Id FROM Games WHERE Name = 'Bali')
	DECLARE @UserGameId INT = (SELECT Id FROM UsersGames WHERE UserId = @UserId AND GameId = @GameId)
	DECLARE @UserGameLevel INT = (SELECT Level FROM UsersGames WHERE Id = @UserGameId)

	DECLARE @counter INT = 251

	WHILE(@counter <= 539)
	BEGIN
		DECLARE @ItemId INT = @counter
		DECLARE @ItemPrice MONEY = (SELECT Price FROM Items WHERE Id = @ItemId)
		DECLARE @ItemLevel INT = (SELECT MinLevel FROM Items WHERE Id = @ItemId)
		DECLARE @UserGameCash MONEY = (SELECT Cash FROM UsersGames WHERE Id = @UserGameId)

		IF(@UserGameCash >= @ItemPrice AND @UserGameLevel >= @ItemLevel)
		BEGIN
			UPDATE UsersGames
			SET Cash -= @ItemPrice
			WHERE Id = @UserGameId

			INSERT INTO UserGameItems VALUES
			(@ItemId, @UserGameId)
		END

		SET @counter += 1
		
		IF(@counter = 300)
		BEGIN
			SET @counter = 501
		END
	END
END

EXEC usp_BuyItems 'baleremuda'
EXEC usp_BuyItems 'loosenoise'
EXEC usp_BuyItems 'inguinalself'
EXEC usp_BuyItems 'buildingdeltoid'
EXEC usp_BuyItems 'monoxidecos'
GO

SELECT * FROM Users AS u
JOIN UsersGames AS ug
ON u.Id = ug.UserId
JOIN Games AS g
ON ug.GameId = g.Id
JOIN UserGameItems AS ugi
ON ug.Id = ugi.UserGameId
JOIN Items AS i
ON ugi.ItemId = i.Id
WHERE g.Name = 'Bali'
ORDER BY u.Username, i.Name
GO

--20. *Massive Shopping
DECLARE @UserId INT = (SELECT Id FROM Users WHERE Username = 'Stamat')
DECLARE @GameId INT = (SELECT Id FROM Games WHERE Name = 'Safflower')
DECLARE @UserGameId INT = (SELECT Id FROM UsersGames WHERE UserId = @UserId AND GameId = @GameId)
DECLARE @UserGameLevel INT = (SELECT Level FROM UsersGames WHERE Id = @UserGameId)
DECLARE @ItemStartLevel INT = 11
DECLARE @ItemEndLevel INT = 12
DECLARE @AllItemsPrice MONEY = (SELECT SUM(Price) FROM Items WHERE (MinLevel BETWEEN @ItemStartLevel AND @ItemEndLevel)) 
DECLARE @StamatCash MONEY = (SELECT Cash FROM UsersGames WHERE Id = @UserGameId)

IF(@StamatCash >= @AllItemsPrice)
BEGIN
	BEGIN TRAN	
		UPDATE UsersGames
		SET Cash -= @AllItemsPrice
		WHERE Id = @UserGameId
	
		INSERT INTO UserGameItems
		SELECT i.Id, @UserGameId  FROM Items AS i
		WHERE (i.MinLevel BETWEEN @ItemStartLevel AND @ItemEndLevel)
	COMMIT
END

SET @ItemStartLevel = 19
SET @ItemEndLevel = 21
SET @AllItemsPrice = (SELECT SUM(Price) FROM Items WHERE (MinLevel BETWEEN @ItemStartLevel AND @ItemEndLevel)) 
SET @StamatCash = (SELECT Cash FROM UsersGames WHERE Id = @UserGameId)

IF(@StamatCash >= @AllItemsPrice)
BEGIN
	BEGIN TRAN
		UPDATE UsersGames
		SET Cash -= @AllItemsPrice
		WHERE Id = @UserGameId
	
		INSERT INTO UserGameItems
		SELECT i.Id, @UserGameId  FROM Items AS i
		WHERE (i.MinLevel BETWEEN @ItemStartLevel AND @ItemEndLevel)
	COMMIT
END

SELECT i.Name AS [Item Name] FROM Users AS u
JOIN UsersGames AS ug
ON u.Id = ug.UserId
JOIN Games AS g
ON ug.GameId = g.Id
JOIN UserGameItems AS ugi
ON ug.Id = ugi.UserGameId
JOIN Items AS i
ON ugi.ItemId = i.Id
WHERE u.Username = 'Stamat' AND g.Name = 'Safflower'
ORDER BY i.Name

GO

USE SoftUni

GO

--21. Employees with Three Projects
CREATE PROC usp_AssignProject @emloyeeId INT , @projectID INT 
AS
BEGIN TRAN
	INSERT INTO EmployeesProjects VALUES (@emloyeeId, @projectID)

	IF ((SELECT COUNT(ProjectID) 
		  FROM EmployeesProjects 
		 WHERE EmployeeID = @emloyeeId) > 3)
	BEGIN
		ROLLBACK
		RAISERROR ('The employee has too many projects!', 16, 1)
		RETURN	
	END
COMMIT 

GO

EXEC dbo.usp_AssignProject 4 ,11
EXEC dbo.usp_AssignProject 2 ,11

GO
--22. Delete Employees

CREATE TABLE Deleted_Employees
			 (EmployeeId INT PRIMARY KEY IDENTITY, 
			  FirstName VARCHAR(50) NOT NULL, 
			  LastName VARCHAR (50) NOT NULL, 
			  MiddleName VARCHAR(50), 
			  JobTitle VARCHAR(50), 
			  DepartmentId INT, 
			  Salary MONEY)

GO

CREATE TRIGGER tr_EmplpyeesDelete ON Employees
AFTER DELETE
AS
INSERT INTO Deleted_Employees
SELECT FirstName, LastName, MiddleName, JobTitle, DepartmentID, Salary FROM deleted
