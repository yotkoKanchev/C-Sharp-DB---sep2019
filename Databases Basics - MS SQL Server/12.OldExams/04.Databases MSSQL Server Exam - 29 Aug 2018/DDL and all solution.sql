CREATE DATABASE Supermarket
USE Supermarket

GO

--01. DDL
CREATE TABLE Categories
			 (
				Id INT PRIMARY KEY IDENTITY,
				[Name] NVARCHAR(30) NOT NULL
			 )

CREATE TABLE Items
			 (
				Id INT PRIMARY KEY IDENTITY,
				[Name] NVARCHAR(30) NOT NULL,
				Price DECIMAL(18,2) NOT NULL,
				CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL
			 )

CREATE TABLE Employees
			 (
				Id INT PRIMARY KEY IDENTITY,
				FirstName NVARCHAR(50) NOT NULL,
				LastName NVARCHAR(50) NOT NULL,
				Phone VARCHAR(12) NOT NULL,
				Salary DECIMAL(18,2) NOT NULL
			 )

CREATE TABLE Orders
			 (
				Id INT PRIMARY KEY IDENTITY,
				[DateTime] DATETIME NOT NULL,
				EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
			 )

CREATE TABLE OrderItems
			 (
				OrderId INT FOREIGN KEY REFERENCES Orders(Id) NOT NULL,
				ItemId INT FOREIGN KEY REFERENCES Items(Id) NOT NULL,
				Quantity INT NOT NULL,
				CHECK (Quantity >= 1),
				CONSTRAINT PK_OrdersItems
				PRIMARY KEY (OrderId, ItemId)
			 )



CREATE TABLE Shifts
			 (
				Id INT IDENTITY NOT NULL,
				EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
				CheckIn DATETIME NOT NULL,
				CheckOut DATETIME NOT NULL,
				CHECK(CheckOut > CheckIn),
				CONSTRAINT PK_EmployeesShifts
				PRIMARY KEY (Id, EmployeeID)
			 )

GO

--02. Insert
INSERT INTO Employees (FirstName, LastName, Phone, Salary) VALUES
('Stoyan', 'Petrov', '888-785-8573', 500.25),
('Stamat', 'Nikolov', '789-613-1122', 999995.25),
('Evgeni', 'Petkov', '645-369-9517', 1234.51),
('Krasimir', 'Vidolov', '321-471-9982', 50.25)

INSERT INTO Items ([Name], Price, CategoryId) VALUES
('Tesla battery', 154.25, 8),
('Chess', 30.25, 8),
('Juice', 5.32, 1),
('Glasses', 10, 8),
('Bottle of water', 1, 1)

GO

--03. Update
UPDATE Items
   SET Price *= 1.27
 WHERE CategoryId IN (1,2,3)

GO

--04. Delete
DELETE FROM OrderItems
WHERE OrderId = 48

GO

--05. Richest People
  SELECT Id, FirstName 
    FROM Employees
   WHERE Salary > 6500
ORDER BY FirstName, Id

GO

--06. Cool Phone Numbers
  SELECT CONCAT(FirstName, ' ', LastName) AS [Full Name], 
		 Phone AS [Phone Number] 
    FROM Employees
   WHERE Phone LIKE '3%'
ORDER BY FirstName, Phone

GO

--07. Employee Statistics
  SELECT e.FirstName, e.LastName, COUNT(o.Id) AS [Count]
    FROM Employees AS e
    JOIN Orders AS o ON o.EmployeeId = e.Id
GROUP BY e.FirstName, e.LastName
ORDER BY [Count] DESC, e.FirstName

GO

--08. Hard Workers Club
  SELECT e.FirstName, e.LastName, AVG(DATEDIFF(HOUR, s.CheckIn, s.CheckOut)) AS [Work Hours]
    FROM Employees AS e
    JOIN Shifts AS s ON s.EmployeeId = e.Id
GROUP BY e.FirstName, e.LastName, e.Id
  HAVING AVG(DATEDIFF(HOUR, s.CheckIn, s.CheckOut)) > 7
ORDER BY [Work Hours] DESC, e.Id

GO

--09. The Most Expensive Order
SELECT TOP(1) o.Id ,SUM(i.Price * oi.Quantity) AS [TotalPrice]
         FROM Orders AS o
         JOIN OrderItems AS oi ON oi.OrderId = o.Id
         JOIN Items AS i ON oi.ItemId = i.Id
     GROUP BY o.Id
     ORDER BY [TotalPrice] DESC

GO

--10. Rich Item, Poor Item
SELECT TOP(10) o.Id AS OrderId,
			   MAX(i.Price) AS [ExpensivePrice], MIN(i.Price) AS [CheapPrice]
         FROM Orders AS o
         JOIN OrderItems AS oi ON oi.OrderId = o.Id
         JOIN Items AS i ON oi.ItemId = i.Id
     GROUP BY o.Id
     ORDER BY [ExpensivePrice] DESC, o.Id

GO

--11. Cashiers
  SELECT e.Id, e.FirstName, e.LastName
    FROM Employees AS e
	JOIN Orders AS o ON o.EmployeeId = e.Id
GROUP BY e.Id, e.FirstName, e.LastName
ORDER BY e.Id
	--b
	SELECT DISTINCT e.Id, e.FirstName, e.LastName
			   FROM Employees AS e
			   JOIN Orders AS o ON o.EmployeeId = e.Id
           ORDER BY e.Id

GO

--12. Lazy Employees
  SELECT DISTINCT e.Id, CONCAT(e.FirstName, ' ', e.LastName) AS [Full Name]
    FROM Employees AS e
    JOIN Shifts AS s ON s.EmployeeId = e.Id
   WHERE DATEDIFF(HOUR, s.CheckIn, s.CheckOut) < 4
ORDER BY e.Id

GO

--13. Sellers
SELECT TOP(10) 
			  CONCAT(e.FirstName, ' ', e.LastName) AS [Full Name], 
			  SUM(gr.TotalSum) AS [Total Price], 
			  SUM(gr.Count) AS [Items] 
		 FROM
			  ( SELECT oi.OrderId, SUM(i.Price * oi.Quantity) AS TotalSum, SUM(oi.Quantity) AS [Count]
			      FROM OrderItems AS oi
			      JOIN Items AS i ON oi.ItemId = i.Id
			  GROUP BY oi.OrderId) AS gr
	     JOIN Orders AS o ON gr.OrderId = o.Id
	     JOIN Employees AS e ON o.EmployeeId = e.Id
	    WHERE o.[DateTime] < '2018-06-15'
	 GROUP BY e.FirstName, e.LastName
	 ORDER BY [Total Price] DESC, [Items] DESC

GO

--14. Tough Days
   SELECT CONCAT(e.FirstName, ' ', e.LastName) AS [Full Name], FORMAT(s.CheckIn, 'dddd') AS [Day of week]
     FROM Employees AS e
LEFT JOIN Orders AS o ON o.EmployeeId = e.Id
LEFT JOIN Shifts AS s ON s.EmployeeId = e.Id
    WHERE DATEDIFF(HOUR, s.CheckIn, s.CheckOut) > 12 AND o.EmployeeId IS NULL
 ORDER BY e.Id

GO

--15. Top Order per Employee - NOT FINISHED YET !!!
SELECT DISTINCT CONCAT(e.FirstName, ' ', e.LastName), DATEDIFF(HOUR, s.CheckIn, s.CheckOut), ranked.TotalPrice
FROM Employees AS E
JOIN (SELECT o.EmployeeId, gro.OrderPrice AS TotalPrice, 
DENSE_RANK() OVER (PARTITION BY o.EmployeeId ORDER BY gro.OrderPrice DESC) AS [Rank]
FROM Orders AS o
JOIN
		(SELECT oi.OrderId, SUM(i.Price * oi.Quantity) AS OrderPrice
		  FROM OrderItems AS oi
		  JOIN Items AS i ON oi.ItemId = i.Id
		  GROUP BY oi.OrderId) AS gro ON gro.OrderId = o.id) AS ranked ON ranked.EmployeeId = e.Id
JOIN Shifts AS s ON s.EmployeeId = e.Id
WHERE ranked.Rank = 1

GO

--16. Average Profit per Day
  SELECT DATEPART(day, o.[DateTime]) AS [Day], FORMAT(AVG(i.Price * oi.Quantity), 'N2') AS [Total profit]
    FROM Orders AS o
    JOIN OrderItems AS oi ON oi.OrderId = o.Id
    JOIN Items AS i ON oi.ItemId = i.Id
GROUP BY DATEPART(day, o.[DateTime])
ORDER BY [DAY]

GO

--17. Top Products
  SELECT i.[Name] AS [Item], c.[Name] AS [Category], SUM(oi.Quantity) AS [Count], SUM(i.Price * oi.Quantity) AS [TotalPrice] 
    FROM Items AS i
    LEFT JOIN Categories AS c ON i.CategoryId = c.Id
    LEFT JOIN OrderItems AS oi ON oi.ItemId = i.Id
GROUP BY i.[Name], c.[Name]
ORDER BY [TotalPrice] DESC, [Count] DESC

GO

--18. Promotion Days
CREATE FUNCTION udf_GetPromotedProducts(@currentDate DATETIME, @startDate DATETIME, @endDate DATETIME, @discount DECIMAL(5,2), @firstItemId INT, @secondItemId INT, @thirdItemId INT)
RETURNS NVARCHAR(max)
AS
BEGIN
	DECLARE @itemsCount INT = (SELECT COUNT(*) FROM Items WHERE Id IN (@firstItemId, @secondItemId, @thirdItemId))

	IF @itemsCount < 3
	BEGIN
		RETURN 'One of the items does not exists!'
	END

	DECLARE @startDateDiff INT = DATEDIFF(SECOND, @startDate, @currentDate)
	DECLARE @endDateDiff INT = DATEDIFF(SECOND, @currentDate, @endDate)

	IF(@startDateDiff <= 0 OR @endDateDiff <= 0)
	BEGIN
		RETURN 'The current date is not within the promotion dates!'
	END 

	DECLARE @firstItemName VARCHAR(30) = (SELECT [Name] FROM Items WHERE Id = @firstItemId)
	DECLARE @secondItemName VARCHAR(30) = (SELECT [Name] FROM Items WHERE Id = @secondItemId)
	DECLARE @thirdItemName VARCHAR(30) = (SELECT [Name] FROM Items WHERE Id = @thirdItemId) 
	DECLARE @firstItemPrice DECIMAL(18,2) = (SELECT Price FROM Items WHERE Id = @firstItemId) * (1 - @discount / 100)
	DECLARE @secondItemPrice DECIMAL(18,2) = (SELECT Price FROM Items WHERE Id = @secondItemId) * (1 - @discount / 100)
	DECLARE @thirdItemPrice DECIMAL(18,2) = (SELECT Price FROM Items WHERE Id = @thirdItemId) * (1 - @discount / 100)

	RETURN CONCAT(@firstItemName, ' price: ', @firstItemPrice, ' <-> ', @secondItemName, ' price: ', @secondItemPrice, ' <-> ', @thirdItemName, ' price: ', @thirdItemPrice )
END

GO

SELECT dbo.udf_GetPromotedProducts('2018-08-02', '2018-08-01', '2018-08-03',13, 3,4,5)

GO

--19. Cancel Order
CREATE PROC usp_CancelOrder @orderId INT, @cancelDate DATETIME
AS
BEGIN
	DECLARE @order INT = (SELECT Id FROM Orders WHERE Id = @OrderId)

	IF (@order IS NULL)
	BEGIN
		RAISERROR ('The order does not exist!', 16, 1)
		RETURN
	END

	DECLARE @dateDiff INT = (SELECT DATEDIFF(DAY, [DateTime], @cancelDate) FROM Orders WHERE Id = @orderId)

	IF(@dateDiff > 3)
	BEGIN
		RAISERROR ('You cannot cancel the order!', 16, 1)
		RETURN
	END
	DELETE FROM OrderItems
	WHERE OrderId = @orderId

	DELETE FROM Orders
	WHERE Id = @orderId
END

GO

EXEC usp_CancelOrder 1, '2018-06-02'
SELECT COUNT(*) FROM Orders
SELECT COUNT(*) FROM OrderItems
EXEC usp_CancelOrder 1, '2018-06-15'
EXEC usp_CancelOrder 124231, '2018-06-15'

GO

--20. Deleted Orders
CREATE TABLE DeletedOrders
			 (OrderId INT,
			  ItemId INT,
			  ItemQuantity INT)

GO

CREATE TRIGGER tr_OrdersDeleted ON OrderItems
AFTER DELETE
AS
BEGIN
	INSERT INTO DeletedOrders 
	SELECT d.OrderId, d.ItemId, d.Quantity FROM deleted AS d
END

GO

DELETE FROM OrderItems
WHERE OrderId = 5

DELETE FROM Orders
WHERE Id = 5 


