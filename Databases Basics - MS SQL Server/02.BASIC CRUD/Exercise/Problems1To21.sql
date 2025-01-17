USE SoftUni

SELECT * FROM Departments

SELECT [Name] FROM Departments

GO

SELECT FirstName, LastName, Salary FROM Employees

GO

SELECT FirstName, MiddleName, LastName FROM Employees

GO

SELECT FirstName, MiddleName, LastName FROM Employees

GO

SELECT FirstName + '.' + LastName + '@softuni.bg' AS [Full Email Address]
From Employees

GO

SELECT DISTINCT Salary FROM Employees

GO 

SELECT * FROM Employees
WHERE JobTitle = 'Sales Representative'

GO

SELECT FirstName, LastName, JobTitle FROM Employees
WHERE Salary BETWEEN 20000 AND 30000

GO

SELECT CONCAT(FirstName,' ', MiddleName, ' ', LastName) AS [Full Name]
FROM Employees
WHERE Salary IN (25000, 14000, 12500, 23600)

GO

SELECT FirstName, LastName FROM Employees
WHERE ManagerID IS NULL

GO

SELECT FirstName, LastName, Salary FROM Employees
WHERE Salary >= 50000
ORDER BY Salary DESC

GO

SELECT TOP(5) FirstName, LastName FROM Employees
ORDER BY Salary DESC

GO

SELECT FirstName, LastName FROM Employees
WHERE DepartmentID <> 4

GO

SELECT * FROM Employees
ORDER BY Salary DESC, FirstName, LastName DESC, MiddleName

GO

CREATE VIEW V_EmployeesSalaries AS
	 SELECT FirstName, LastName, Salary 
	   FROM Employees

GO

CREATE VIEW V_EmployeeNameJobTitle  AS
	 SELECT CONCAT(FirstName,' ', MiddleName, ' ', LastName) AS [Full Name] , JobTitle AS [Job Title]
	   FROM Employees

GO

SELECT DISTINCT JobTitle FROM Employees

GO

SELECT TOP(10) * FROM Projects
WHERE StartDate IS NOT NULL
ORDER BY StartDate, [Name]

GO

SELECT TOP(7) FirstName, LastName, HireDate FROM Employees
ORDER BY HireDate DESC
 
GO

UPDATE Employees
SET Salary *= 1.12
WHERE DepartmentID IN (1,2,4,11)

SELECT Salary FROM Employees


 