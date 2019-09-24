USE SoftUni

--1
SELECT FirstName, LastName
  FROM Employees
 WHERE FirstName LIKE 'SA%'

GO

--2
SELECT FirstName, LastName
  FROM Employees
 WHERE LastName LIKE '%ei%'

GO

--3
SELECT FirstName 
  FROM Employees
 WHERE DepartmentID IN (3,10) AND DATEPART(YEAR, HireDate) BETWEEN 1995 AND 2005

GO

--4
SELECT FirstName, LastName 
  FROM Employees
 WHERE JobTitle NOT LIKE '%engineer%'

GO

--5
  SELECT [Name] 
    FROM Towns
   WHERE LEN([Name]) IN (5,6)
ORDER BY [Name]

--SELECT [Name] FROM Towns WHERE LEN([Name]) BETWEEN 5 AND 6 ORDER BY [Name]

GO

--6
  SELECT TownID, [Name] 
    FROM Towns
   WHERE LEFT([NAME], 1) IN ('M', 'K', 'B', 'E') 
ORDER BY [Name]

--SELECT * FROM Towns
--WHERE Name LIKE '[MKBE]%'
--ORDER BY Name

GO

--7
SELECT TownID, [Name] 
    FROM Towns
   WHERE LEFT([NAME], 1) NOT IN ('R', 'B', 'D') 
ORDER BY [Name]

GO

--8
CREATE VIEW V_EmployeesHiredAfter2000 AS
SELECT FirstName, LastName 
  FROM Employees
 WHERE DATEPART(YEAR, HireDate) > 2000

GO

--9
SELECT FirstName, LastName 
  FROM Employees
 WHERE LEN(LastName) = 5

 GO

 --10
  SELECT EmployeeID, FirstName, LastName, Salary,
	DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeID) AS Rank
    FROM Employees
   WHERE Salary BETWEEN 10000 AND 50000
ORDER BY Salary DESC

GO

--11	
SELECT * FROM 
(
	SELECT e.EmployeeID, e.FirstName, e.LastName, e.Salary,
		   DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeId) AS Rank
	  FROM Employees AS e
	 WHERE Salary BETWEEN 10000 AND 50000
)      
		   AS tempTable
		WHERE tempTable.Rank = 2
	 ORDER BY Salary DESC