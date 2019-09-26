USE SoftUni

--01. Find Names of All Employees by First Name
	--a
SELECT FirstName, LastName
  FROM Employees
 WHERE FirstName LIKE 'SA%'

	--b
SELECT FirstName, LastName
  FROM Employees
 WHERE LEFT(FirstName, 2) = 'SA' 

--02. Find Names of All Employees by Last Name
SELECT FirstName, LastName 
  FROM Employees
 WHERE LastName LIKE '%ei%'

--03. Find First Names of All Employess
	--a
SELECT FirstName 
  FROM Employees
 WHERE DepartmentID IN (3,10) AND 
	   DATEPART(YEAR, HireDate) BETWEEN 1995 AND 2005

	--b
SELECT FirstName 
  FROM Employees
 WHERE DepartmentID = 3 OR DepartmentID = 10 AND 
	   DATEPART(YEAR, HireDate) >= 1995 AND DATEPART(YEAR, HireDate) <= 2005

--04. Find All Employees Except Engineers
	--a
SELECT FirstName, LastName
  FROM Employees
 WHERE JobTitle NOT LIKE '%engineer%'

--05. Find Towns with Name Length
	--a
  SELECT [Name] 
    FROM Towns
   WHERE LEN([Name]) BETWEEN 5 AND 6
ORDER BY [Name]

	--b
  SELECT [Name] 
    FROM Towns
   WHERE LEN([Name]) = 5 OR LEN([Name]) = 6
ORDER BY [Name]

--06. Find Towns Starting With
	--a
  SELECT TownID, [Name] 
    FROM Towns
   WHERE [Name] LIKE '[MKBE]%'
ORDER BY [Name]

	--b
  SELECT TownID, [Name] 
    FROM Towns
   WHERE LEFT([Name], 1) IN ('M', 'K', 'B', 'E')
ORDER BY [Name]

--07. Find Towns Not Starting With
	--a
  SELECT TownID, [Name] 
    FROM Towns
   WHERE [Name] NOT LIKE '[RBD]%'
ORDER BY [Name]

	--b
  SELECT TownID, [Name] 
    FROM Towns
   WHERE LEFT([Name], 1) NOT IN ('R', 'B', 'D')
ORDER BY [Name]

--08. Create View Employees Hired After
	--a
CREATE VIEW V_EmployeesHiredAfter2000 AS
     SELECT FirstName, LastName 
       FROM Employees
      WHERE DATEPART(YEAR, HireDate) > 2000

	--b
CREATE VIEW V_EmployeesHiredAfter2000 AS
     SELECT FirstName, LastName 
       FROM Employees
      WHERE YEAR(HireDate) > 2000

--09. Length of Last Name
SELECT FirstName, LastName 
  FROM Employees
 WHERE LEN(LastName) = 5

--10. Rank Employees by Salary
  SELECT EmployeeID, FirstName, LastName, Salary,
  		 DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeID) AS [Rank] 
    FROM Employees AS tempTable
   WHERE tempTable.Salary BETWEEN 10000 AND 50000
ORDER BY tempTable.Salary DESC

--11. Find All Employees with Rank 2
SELECT * FROM
(
	SELECT EmployeeID, FirstName, LastName, Salary,
  	       DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeID) AS [Rank] 
      FROM Employees 
     WHERE Salary BETWEEN 10000 AND 50000
)       AS tempTable
    	WHERE tempTable.[Rank] = 2
     ORDER BY tempTable.Salary DESC
