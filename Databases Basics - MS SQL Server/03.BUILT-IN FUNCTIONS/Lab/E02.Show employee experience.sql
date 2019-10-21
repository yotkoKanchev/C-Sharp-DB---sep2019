USE SoftUni

GO

SELECT EmployeeID,FirstName, LastName, 
	   DATEDIFF(YEAR, HireDate, GETDATE())
	AS [Years in Service]	
  FROM Employees