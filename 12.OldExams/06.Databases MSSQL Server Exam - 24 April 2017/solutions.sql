CREATE DATABASE WMS
USE WMS

GO

--01. DDL
CREATE TABLE Clients
	(
	ClientId INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Phone CHAR(12) NOT NULL,
	CHECK (LEN(Phone) = 12)
	)

CREATE TABLE Mechanics
	(
	MechanicId INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	[Address] VARCHAR(255) NOT NULL,
	)

CREATE TABLE Models
	(
	ModelId INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) UNIQUE NOT NULL
	)

CREATE TABLE Vendors
	(
	VendorId INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) UNIQUE NOT NULL
	)

CREATE TABLE Parts
	(
	PartId INT PRIMARY KEY IDENTITY,
	SerialNumber VARCHAR(50) UNIQUE NOT NULL,
	[Description] VARCHAR(255),
	Price MONEY NOT NULL,
	CHECK(Price > 0),
	VendorId INT FOREIGN KEY REFERENCES Vendors(VendorId),
	StockQty INT DEFAULT 0 NOT NULL,
	CHECK (StockQty >= 0)
	)

CREATE TABLE Jobs
	(
	JobId INT PRIMARY KEY IDENTITY,
	ModelId INT FOREIGN KEY REFERENCES Models(ModelId),
	[Status] VARCHAR(11) DEFAULT 'Pending' NOT NULL,
	CHECK ([Status] IN ('Pending', 'In Progress', 'Finished')),
	ClientId INT FOREIGN KEY REFERENCES Clients(ClientId),
	MechanicId INT FOREIGN KEY REFERENCES MEchanics(MechanicId),
	IssueDate DATE NOT NULL,
	FinishDate DATE
	)

CREATE TABLE Orders
	(
	OrderId INT PRIMARY KEY IDENTITY,
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId),
	IssueDate DATE,
	Delivered BIT DEFAULT 0 NOT NULL
	)

CREATE TABLE OrderParts
	(
	OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
	PartId INT FOREIGN KEY REFERENCES Parts(PartId),
	Quantity INT DEFAULT 1 NOT NULL,
	CHECK (Quantity > 0),
	CONSTRAINT PK_OrdersParts
	PRIMARY KEY (OrderId, PartId)
	)

CREATE TABLE PartsNeeded
	(
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId),
	PartId INT FOREIGN KEY REFERENCES Parts(PartId),
	Quantity INT DEFAULT 1 NOT NULL,
	CHECK (Quantity > 0),
	CONSTRAINT PK_JobsParts
	PRIMARY KEY (JobId, PartId)
	)

GO

--02. Insert
INSERT INTO Clients (FirstName, LastName, Phone)
VALUES
('Teri',	'Ennaco',	'570-889-5187'),
('Merlyn',	'Lawler',	'201-588-7810'),
('Georgene','Montezuma','925-615-5185'),
('Jettie',	'Mconnell',	'908-802-3564'),
('Lemuel',	'Latzke',	'631-748-6479'),
('Melodie',	'Knipp',	'805-690-1682'),
('Candida',	'Corbley',	'908-275-8357')

INSERT INTO Vendors ([Name])
VALUES
('Suzhou Precision Products'),
('Shenzhen Ltd.'),
('Fenghua Import Export'),
('Qingdao Technology')


INSERT INTO Parts (SerialNumber, Description, Price, VendorId)
VALUES
('WP8182119', 'Door Boot Seal',	117.86, (SELECT TOP(1) VendorId FROM Vendors WHERE [Name] = 'Suzhou Precision Products')),
('W10780048', 'Suspension Rod',	42.81,(SELECT TOP(1) VendorId FROM Vendors WHERE [Name] = 'Shenzhen Ltd.')),
('W10841140', 'Silicone Adhesive', 	6.77,(SELECT TOP(1) VendorId FROM Vendors WHERE [Name] = 'Fenghua Import Export')),
('WPY055980', 'High Temperature Adhesive',	13.94,(SELECT TOP(1) VendorId FROM Vendors WHERE [Name] = 'Qingdao Technology'))


GO

--03. Update
UPDATE Jobs
   SET MechanicId = 3, [Status] = 'In Progress'
 WHERE [Status] = 'Pending'

GO

--04. Delete
DELETE FROM OrderParts
WHERE OrderId = 19

DELETE FROM Orders
WHERE OrderId = 19

GO

--05. Clients by Name
  SELECT FirstName, LastName, Phone
    FROM Clients
ORDER BY LastName, ClientId

GO

--06. Job Status
   SELECT [Status] , IssueDate
    FROM Jobs
   WHERE [Status] <> 'Finished'
ORDER BY IssueDate, JobId

GO

--07. Mechanic Assignments
  SELECT CONCAT(m.FirstName, ' ', m.LastName) AS [Mechanic], j.[Status], j.IssueDate
    FROM Mechanics AS m
    JOIN Jobs AS j ON j.MechanicId = m.MechanicId
ORDER BY j.MechanicId, j.IssueDate, j.JobId

GO

--08. Current Clients
  SELECT CONCAT(c.FirstName, ' ', c.LastName) AS [Client], 
         DATEDIFF(DAY,j.IssueDate, '2017-04-24') AS [Days going], 
		 j.[Status]
    FROM Clients AS c
    JOIN Jobs AS j ON j.ClientId = c.ClientId
   WHERE j.[Status] <> 'Finished'
ORDER BY [Days going] DESC, c.ClientId

GO

--09. Mechanic Performance
  SELECT CONCAT(m.FirstName, ' ', m.LastName) AS [Mechanic], 
		 AVG(DATEDIFF(DAY,j.IssueDate, j.FinishDate)) AS [Average Days]
    FROM Mechanics AS m
    JOIN Jobs AS j ON j.MechanicId = m.MechanicId
   WHERE j.[Status] = 'Finished'
GROUP BY m.FirstName, m.LastName, m.MechanicId
ORDER BY m.MechanicId

GO

--10. Hard Earners
SELECT TOP(3) CONCAT(m.FirstName, ' ', m.LastName) AS [Mechanic],
			  COUNT(j.JobId) AS Jobs
      FROM Mechanics AS m
      JOIN Jobs AS j ON j.MechanicId = m.MechanicId
     WHERE j.[Status] <> 'Finished'
  GROUP BY m.FirstName, m.LastName, m.MechanicId
  HAVING COUNT(j.JobId) > 1
  ORDER BY [Jobs] DESC, m.MechanicId

GO

--11. Available Mechanics
SELECT CONCAT(m.FirstName, ' ', m.LastName) AS Available
  FROM Mechanics AS m
  LEFT JOIN Jobs AS j ON j.MechanicId = m.MechanicId
  WHERE j.JobId IS NULL OR m.MechanicId NOT IN (SELECT MechanicId FROM Jobs WHERE Status <> 'Finished' AND MechanicId IS NOT NULL)
  GROUP BY m.FirstName, m.LastName, m.MechanicId
  ORDER BY m.MechanicId

GO

--12. Parts Cost
 SELECT ISNULL(SUM(p.Price * op.Quantity),0)  AS [Parts Total]
   FROM Parts AS p
   JOIN OrderParts AS op ON op.PartId = p.PartId
   JOIN Orders AS o ON op.OrderId = o.OrderId
  WHERE DATEADD(WEEK, 3, o.IssueDate) >= '2017-04-24'

GO

--13. Past Expenses
   SELECT j.JobId, ISNULL(SUM(p.Price * op.Quantity),0) AS Total
     FROM Jobs AS j
LEFT JOIN Orders AS o ON o.JobId = j.JobId
LEFT JOIN OrderParts AS op ON op.OrderId = o.OrderId
LEFT JOIN Parts AS p ON op.PartId = p.PartId
    WHERE j.[Status] = 'Finished'
 GROUP BY j.JobId
 ORDER BY Total DESC, j.JobId

GO

--14. Model Repair Time
  SELECT m.ModelId, m.[Name], CAST(AVG(DATEDIFF(DAY, j.IssueDate, j.FinishDate)) AS VARCHAR(20)) + ' days' AS [Average Service Time]
    FROM Models AS m
    JOIN Jobs AS j ON j.ModelId = m.ModelId
GROUP BY m.ModelId, m.[Name]
ORDER BY AVG(DATEDIFF(DAY, j.IssueDate, j.FinishDate))

GO
--15. Faultiest Model
SELECT TOP (1) WITH TIES m.[Name], COUNT(j.JobId) AS [Times Serviced], ISNULL(SUM(p.Price * op.Quantity), 0) AS [Parts Total] 
  FROM Models AS m
  LEFT JOIN Jobs AS j ON j.ModelId = m.ModelId
  LEFT JOIN Orders AS o ON o.JobId = j.JobId
  LEFT JOIN OrderParts AS op ON op.OrderId = o.OrderId
  LEFT JOIN Parts AS p ON op.PartId = p.PartId
  WHERE j.Status = 'Finished'
  GROUP BY m.[Name]
  ORDER BY [Times Serviced] DESC

GO
--16. Missing Parts TODO......
SELECT * 
  FROM Parts AS p
  JOIN OrderParts AS op ON op.PartId = p.PartId
  JOIN Orders AS o ON op.OrderId = o.OrderId
  JOIN Jobs AS j ON o.JobId = j.JobId




  

