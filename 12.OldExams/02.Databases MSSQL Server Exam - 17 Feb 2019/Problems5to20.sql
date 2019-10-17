--05. Teen Students
  SELECT FirstName, LastName, Age 
    FROM Students
   WHERE Age >= 12
ORDER BY FirstName, LastName

GO

--06. Cool Addresses
  SELECT CONCAT(FirstName, ' ', MiddleName, ' ', LastName) AS [Full Name], [Address] 
    FROM Students 
   WHERE [Address] LIKE '%road%'
ORDER BY FirstName, LastName, [Address]

--SELECT CONCAT_WS(' ',FirstName, MiddleName, LastName) AS [Full Name], [Address] 
--    FROM Students 
--   WHERE [Address] LIKE '%road%'
--ORDER BY FirstName, LastName, [Address]
GO

--07. 42 Phones
  SELECT FirstName, [Address], Phone 
	FROM Students
   WHERE MiddleName IS NOT NULL AND Phone LIKE '42%'
ORDER BY FirstName

GO

--08. Students Teachers
  SELECT s.FirstName, s.LastName, COUNT(st.TeacherId)
    FROM Students AS s
    JOIN StudentsTeachers AS st ON st.StudentId = s.Id
GROUP BY s.FirstName, s.LastName

GO

--09. Subjects with Students
SELECT CONCAT(t.FirstName, ' ', t.LastName) AS [Full Name],
	   CONCAT(s.[Name], '-', s.Lessons) AS Subjects,
	   (SELECT COUNT(StudentId) 
		  FROM StudentsTeachers
		 WHERE TeacherId = t.Id) AS Students
  FROM Teachers AS t
  JOIN Subjects AS s ON t.SubjectId = s.Id
ORDER BY Students DESC, [Full Name], Subjects

GO

--10. Students to Go
   SELECT CONCAT(s.FirstName, ' ', s.LastName) AS [Full Name]
     FROM Students AS s
LEFT JOIN StudentsExams AS se ON se.StudentId = s.Id
    WHERE se.ExamId IS NULL
 ORDER BY [Full Name]

GO

--11. Busiest Teachers
  SELECT TOP(10) 
			 t.FirstName, t.LastName, COUNT(st.StudentId) AS [StudentsCount]
        FROM Teachers AS t
        JOIN StudentsTeachers AS st ON st.TeacherId = t.Id
    GROUP BY t.FirstName, t.LastName
    ORDER BY [StudentsCount] DESC, t.FirstName, t.LastName

GO

--12. Top Students
SELECT TOP(10)
           s.FirstName, s.LastName, FORMAT(AVG(se.Grade), 'N2') AS [Grade]
      FROM Students AS s
      JOIN StudentsExams AS se ON se.StudentId = s.Id
  GROUP BY s.FirstName, s.LastName
  ORDER BY [Grade] DESC, s.FirstName, s.LastName

GO

--13. Second Highest Grade
  SELECT r.FirstName, r.LastName, r.Grade 
    FROM (
          SELECT s.FirstName, s.LastName, ss.Grade, 
                 ROW_NUMBER() OVER (PARTITION BY s.Id ORDER BY ss.Grade DESC) AS [Rank]
            FROM Students AS s
            JOIN StudentsSubjects AS ss ON ss.StudentId = s.Id) AS r
   WHERE r.[Rank] = 2
ORDER BY r.FirstName, r.LastName

GO

--14. Not So In The Studying
-- SELECT CONCAT(s.FirstName, ' ', 'MiddleName'  +  ' ', 'LastName') AS [Full Name]
   SELECT IIF(s.MiddleName IS NULL, CONCAT(s.FirstName, ' ', s.LastName), CONCAT(s.FirstName, ' ', s.MiddleName, ' ', s.LastName)) AS [Full Name] 
     FROM Students AS s
LEFT JOIN StudentsSubjects AS ss ON ss.StudentId = s.Id
    WHERE ss.SubjectId IS NULL  
 ORDER BY [Full Name]

GO

--15. Top Student per Teacher
--No Thank you !!!

--16. Average Grade per Subject
  SELECT s.[Name], AVG(ss.Grade) AS AverageGrade
    FROM Subjects AS s
    JOIN StudentsSubjects AS ss ON ss.SubjectId = s.Id
GROUP BY s.[Name], s.id
ORDER BY s.Id

GO

--17. Exams Information
SELECT  dev.[Quarter], dev.SubjectName, COUNT(dev.StudentId) AS StudentsCount
  FROM (
  SELECT s.[Name] AS SubjectName,
		 se.StudentId,
		 CASE
		 WHEN DATEPART(MONTH, Date) BETWEEN 1 AND 3 THEN 'Q1'
		 WHEN DATEPART(MONTH, Date) BETWEEN 4 AND 6 THEN 'Q2'
		 WHEN DATEPART(MONTH, Date) BETWEEN 7 AND 9 THEN 'Q3'
		 WHEN DATEPART(MONTH, Date) BETWEEN 10 AND 12 THEN 'Q4'
		 WHEN Date IS NULL THEN 'TBA'
		 END AS [Quarter]
    FROM Exams AS e
	JOIN Subjects AS s ON s.Id = e.SubjectId 
	JOIN StudentsExams AS se ON se.ExamId = e.Id
	WHERE se.Grade >= 4
) AS dev
GROUP BY dev.[Quarter], dev.SubjectName
ORDER BY dev.[Quarter]

GO

--18. Exam Grades
CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT , @grade DECIMAL(3,2))
RETURNS NVARCHAR(100)
AS
BEGIN
	DECLARE @studentName NVARCHAR(30) = (SELECT FirstName 
										   FROM Students 
										  WHERE Id = @studentId)
	IF (@studentName IS NULL)
	BEGIN
		RETURN 'The student with provided id does not exist in the school!'
	END

	IF(@grade > 6.00)
	BEGIN
		RETURN 'Grade cannot be above 6.00!'
	END

	DECLARE @examGradesCount INT = (SELECT COUNT(ExamId)
									  FROM StudentsExams 
									 WHERE StudentId = @studentId AND Grade BETWEEN @grade AND @grade + 0.5)

	RETURN CONCAT('You have to update ', @examGradesCount,  ' grades for the student ', @studentName )
END

GO
--
SELECT dbo.udf_ExamGradesToUpdate(12, 5.50)

GO

--19. Exclude From School
CREATE PROC usp_ExcludeFromSchool @StudentId INT
AS
BEGIN
	DECLARE @studentName NVARCHAR(30) = (SELECT FirstName 
										   FROM Students 
										   WHERE Id = @StudentId)

	IF (@studentName IS NULL) 
	BEGIN
		RAISERROR('This school has no student with the provided id!', 16, 1)
		RETURN
	END

	DELETE FROM StudentsExams
	WHERE StudentId = @StudentId

	DELETE FROM StudentsSubjects
	WHERE StudentId = @StudentId

	DELETE FROM StudentsTeachers
	WHERE StudentId = @StudentId

	DELETE FROM Students
	WHERE Id = @StudentId
END
--
EXEC usp_ExcludeFromSchool 1
SELECT COUNT(*) FROM Students

GO

--20. Deleted Students
CREATE TABLE ExcludedStudents
			 (
				StudentId INT PRIMARY KEY,
				StudentName NVARCHAR(100) NOT NULL
			 )

GO

CREATE TRIGGER TR_DeletedStudents ON Students
AFTER DELETE 
AS
--
INSERT INTO ExcludedStudents
SELECT d.Id, CONCAT(d.FirstName, ' ', d.LastName) FROM deleted AS d

DELETE FROM StudentsExams
WHERE StudentId = 1

DELETE FROM StudentsTeachers
WHERE StudentId = 1

DELETE FROM StudentsSubjects
WHERE StudentId = 1

DELETE FROM Students
WHERE Id = 1

SELECT * FROM ExcludedStudents
