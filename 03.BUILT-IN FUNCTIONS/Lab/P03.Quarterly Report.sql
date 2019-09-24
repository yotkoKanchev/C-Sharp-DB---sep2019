USE Orders

GO

SELECT Id, ProductName,
	   DATEPART(QUARTER,OrderDate) AS Quarter,
	   DATEPART(MONTH, OrderDate) AS Month,
	   DATEPART(YEAR, OrderDate) AS Year,
	   DATEPART(DAY, OrderDate) As Day
  FROM Orders
