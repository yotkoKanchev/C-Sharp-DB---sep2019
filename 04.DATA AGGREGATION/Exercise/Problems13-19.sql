USE SoftUni

--13. Departments Total Salaries
  SELECT DepartmentID,
		 SUM(Salary) AS TotalSalary  
    FROM Employees
GROUP BY DepartmentID
ORDER BY DepartmentID

--14. Employees Minimum Salaries
  SELECT DepartmentID,
		 MIN(Salary) AS MinimumSalary 
    FROM Employees
   WHERE DepartmentID IN (2,5,7) AND HireDate > '2000-01-01'
GROUP BY DepartmentID

--15. Employees Average Salaries
SELECT *
  INTO AggFunNewTable
  FROM Employees
 WHERE Salary > 30000

DELETE FROM AggFunNewTable
	  WHERE ManagerID = 42

UPDATE AggFunNewTable
   SET Salary += 5000
 WHERE DepartmentID = 1

  SELECT DepartmentID,
		 AVG(Salary) AS AverageSalary
    FROM AggFunNewTable
GROUP BY DepartmentID

--16. Employees Maximum Salaries
  SELECT DepartmentID,
		 MAX(Salary) AS MaxSalary
    FROM Employees
GROUP BY DepartmentID
  HAVING Max(Salary) NOT BETWEEN 30000 AND 70000

--17. Employees Count Salaries
SELECT COUNT(EmployeeID) AS [Count]
  FROM Employees
 WHERE ManagerID IS NULL

--18. 3rd Highest Salary
SELECT t.DepartmentID, t.Salary AS ThirdHighestSalary
  FROM
	    (SELECT DepartmentID, 
	            Salary,
  	            ROW_NUMBER() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS [Rank]  
	       FROM Employees
	   GROUP BY DepartmentID, Salary) AS t
 WHERE t.[Rank] = 3

--19. Salary Challenge

SELECT TOP(10) t1.FirstName, t1.LastName, t1.DepartmentID 
      FROM Employees AS t1
     WHERE Salary > 
	    			(SELECT AVG(Salary) AS AvgSalary
	    			   FROM Employees AS t2
	    			  WHERE t2.DepartmentID = t1.DepartmentID)
  ORDER BY DepartmentID




