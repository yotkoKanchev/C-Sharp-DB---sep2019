USE SoftUni

GO

--01. Employees with Salary Above 35000
CREATE PROC usp_GetEmployeesSalaryAbove35000 
AS
	SELECT FirstName AS [First Name], LastName AS [Last Name] 
	  FROM Employees
	 WHERE Salary > 35000

EXEC dbo.usp_GetEmployeesSalaryAbove35000

GO

--02. Employees with Salary Above Number
CREATE PROC usp_GetEmployeesSalaryAboveNumber @salary DECIMAL(18,4)
AS
	SELECT FirstName AS [First Name], LastName AS [Last Name] 
	  FROM Employees
	 WHERE Salary >= @salary

EXEC dbo.usp_GetEmployeesSalaryAboveNumber 35000

GO

--03. Town Names Starting With
CREATE PROC usp_GetTownsStartingWith @string VARCHAR(max)
AS
	SELECT [Name] AS Town
	  FROM Towns
	 WHERE [Name] LIKE @string + '%'

EXEC dbo.usp_GetTownsStartingWith 'b'

GO
--04. Employees from Town
CREATE PROC usp_GetEmployeesFromTown @townName VARCHAR(50)
AS
	SELECT e.FirstName AS [First Name], e.LastName AS [Last Name]
	  FROM Employees AS e
	  JOIN Addresses AS a ON a.AddressID = e.AddressID
	  JOIN Towns AS t ON t.TownID = a.TownID
	 WHERE t.[Name] = @townName

EXEC dbo.usp_GetEmployeesFromTown 'Sofia'

GO

--05. Salary Level Function
CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS VARCHAR(7)
AS
BEGIN
	DECLARE @salaryLevel VARCHAR(7) = 'High'
	IF (@salary < 30000)
		BEGIN
			SET @salaryLevel = 'Low'
		END
	ELSE IF (@salary <= 50000)
		BEGIN
			SET @salaryLevel = 'Average'
		END

	RETURN @salaryLevel
END

GO

SELECT Salary, dbo.ufn_GetSalaryLevel(Salary) AS [Salary Level]	    
  FROM Employees

GO

--06. Employees by Salary Level
CREATE PROC usp_EmployeesBySalaryLevel @salaryLevel VARCHAR(7)
AS

	SELECT FirstName AS [First Name], LastName AS [Last Name] 
	  FROM Employees
	 WHERE @salaryLevel = dbo.ufn_GetSalaryLevel(Salary) 

EXEC dbo.usp_EmployeesBySalaryLevel 'high'

GO

--07. Define Function
CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(max), @word VARCHAR(max)) 
RETURNS BIT
AS
BEGIN
	DECLARE @lengthOfWord INT = LEN(@word)
	DECLARE @index INT = 1

	WHILE (@index <= @lengthOfWord)
		BEGIN
			IF (CHARINDEX(SUBSTRING(@word,@index,1), @setOfLetters) = 0 )
				BEGIN
					RETURN 0
				END
			SET @index += 1
		END
	RETURN 1
END

GO

SELECT dbo.ufn_IsWordComprised('abcdefg', 'abc')

GO

USE SoftUni
GO

--08. Delete Employees and Departments
CREATE PROC usp_DeleteEmployeesFromDepartment @departmentId INT
AS
	DELETE FROM EmployeesProjects 
		  WHERE EmployeeID IN (SELECT EmployeeID 
						         FROM Employees 
						        WHERE DepartmentID = @departmentId)

	UPDATE Employees
	   SET ManagerID = NULL
	 WHERE ManagerID IN (SELECT EmployeeID 
						   FROM Employees 
						  WHERE DepartmentID = @departmentId)

	ALTER TABLE Departments
	ALTER COLUMN ManagerID INT

	UPDATE Departments
	   SET ManagerID = NULL
	 WHERE DepartmentID = @departmentId

	DELETE FROM Employees
		  WHERE DepartmentID = @departmentId

	DELETE FROM Departments
		  WHERE DepartmentID = @departmentId

	SELECT COUNT(*)
	  FROM Employees
	 WHERE DepartmentID = @departmentId

EXEC dbo.usp_DeleteEmployeesFromDepartment 4

GO