USE SoftUni

GO

--01. Employee Address
SELECT TOP(5) 
    	   e.EmployeeID, e.JobTitle, e.AddressID, a.AddressText
      FROM Employees AS e
      JOIN Addresses AS a ON a.AddressID = e.AddressID
  ORDER BY e.AddressID

--02. Addresses with Towns
SELECT TOP(50) 
		   e.FirstName, e.LastName, t.[Name] AS Town, a.AddressText
	  FROM Employees AS e
	  JOIN Addresses AS a ON a.AddressID = e.AddressID
	  JOIN Towns AS t ON t.TownID = a.TownID
  ORDER BY e.FirstName, e.LastName

--03. Sales Employees
  SELECT e.EmployeeID, e.FirstName, e.LastName, d.[Name] AS DepartmentName
    FROM Employees AS e
    JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
   WHERE d.[Name] = 'Sales'
ORDER BY e.EmployeeID

--04. Employee Departments
SELECT TOP(5)
		   e.EmployeeID, e.FirstName, e.Salary, d.[Name] AS DepartmentName  
      FROM Employees AS e
      JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
     WHERE e.Salary > 15000
  ORDER BY e.DepartmentID

--05. Employees Without Projects
 SELECT TOP(3) 
			e.EmployeeID, e.FirstName
       FROM Employees AS e
  LEFT JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
      WHERE ep.EmployeeID IS NULL
   ORDER BY e.EmployeeID

--06. Employees Hired After
  SELECT e.FirstName, e.LastName, e.HireDate, d.[Name] AS DeptName
    FROM Employees AS e
    JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
   WHERE e.HireDate > '1999-01-01' AND d.[Name] IN ('Sales','Finance')
ORDER BY e.HireDate

--07. Employees With Project
SELECT TOP(5)
		   e.EmployeeID, e.FirstName, p.[Name] AS [ProjectName]
      FROM Employees AS e
      JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
      JOIN Projects AS p ON p.ProjectID = ep.ProjectID
     WHERE p.StartDate > '2002-08-13' AND p.EndDate IS NULL
   ORDER BY e.EmployeeID

--08. Employee 24
	--a
  SELECT e.EmployeeID, e.FirstName, p.[Name] AS [ProjectName]
    FROM Employees AS e
    LEFT JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
    LEFT JOIN Projects AS p ON p.ProjectID = ep.ProjectID AND p.StartDate < '2005'
   WHERE e.EmployeeID = 24
ORDER BY e.EmployeeID

	--b
  SELECT e.EmployeeID, e.FirstName,
		 IIF(p.StartDate < '2005', p.[Name], NULL) AS [ProjectName]
    FROM Employees AS e
    LEFT JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
    LEFT JOIN Projects AS p ON p.ProjectID = ep.ProjectID
   WHERE e.EmployeeID = 24
ORDER BY e.EmployeeID

--09. Employee Manager
  SELECT e.EmployeeID, e.FirstName, e.ManagerID, me.FirstName
    FROM Employees AS e
    JOIN Employees AS me ON me.EmployeeID = e.ManagerID
   WHERE e.ManagerID IN (3, 7)
ORDER BY e.EmployeeID

--10. Employees Summary
SELECT TOP(50) 
		   e.EmployeeID,
		   CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName,
		   CONCAT(me.FirstName, ' ', me.LastName) AS DepartmentName,
		   d.[Name] AS DeparmentName
      FROM Employees AS e
      JOIN Employees AS me ON me.EmployeeID = e.ManagerID
      JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
  ORDER BY e.EmployeeID

--11. Min Average Salary
	--a
SELECT TOP(1) 
		   AVG(Salary) AS MinAverageSalary
      FROM Employees
  GROUP BY DepartmentID
  ORDER BY AVG(Salary)

	--b
SELECT MIN(av.averageSalary) AS MinimumAverageSalary
  FROM (SELECT AVG(Salary) AS averageSalary
	      FROM Employees
	  GROUP BY DepartmentID) AS av
