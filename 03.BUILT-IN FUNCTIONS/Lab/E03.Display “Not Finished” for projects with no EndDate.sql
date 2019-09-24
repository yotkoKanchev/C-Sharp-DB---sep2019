USE SoftUni

SELECT * FROM Projects

SELECT ProjectID, Name,
	   ISNULL (
			CAST (EndDate AS VARCHAR), 
			'Not Finished !!!'
			  )
	AS [End Date]
  FROM Projects