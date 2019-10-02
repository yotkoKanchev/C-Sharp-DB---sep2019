CREATE TABLE Students
			 (
				StudentID INT PRIMARY KEY IDENTITY,
				[Name] NVARCHAR(50) NOT NULL
			 )

CREATE TABLE Exams
			 (
				ExamID INT PRIMARY KEY,
				[Name] NVARCHAR(50) NOT NULL
			 )

INSERT INTO Students VALUES
('Mila'),
('Toni'),
('Ron')

INSERT INTO Exams VALUES
(101, 'SpringMVC'),
(102, 'Neo4j'),
(103, 'Oracle 11g')


CREATE TABLE StudentsExams
			 (
				StudentID INT,
				ExamID INT,
				CONSTRAINT PK_StudentsExams
				PRIMARY KEY(StudentID, ExamId),
				CONSTRAINT FK_StudentsExams_Students
				FOREIGN KEY(StudentID) REFERENCES Students(StudentID),
				CONSTRAINT FK_StudentsExams_Exams
				FOREIGN KEY(ExamID) REFERENCES Exams(ExamID) 
			 )

INSERT INTO StudentsExams VALUES 
(1, 101),
(1, 102),
(2, 101),
(3, 103),
(2, 102),
(2, 103)

