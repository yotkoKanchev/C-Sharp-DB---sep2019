--CREATE DATABASE University

--USE University

CREATE TABLE Majors
			 (
				MajorID INT PRIMARY KEY IDENTITY,
				[Name] NVARCHAR(50) NOT NULL
			 )

CREATE TABLE Students
			 (
				StudentID INT PRIMARY KEY IDENTITY,
				StudentNumber NVARCHAR(50) NOT NULL,
				StudentName NVARCHAR(100) NOT NULL,
				MajorID INT NOT NULL,
				CONSTRAINT FK_Students_Major
				FOREIGN KEY (MajorID) REFERENCES Majors(MajorID)
			 )

CREATE TABLE Payments
			 (
				PaymentID INT PRIMARY KEY IDENTITY,
				PaymentDate DATE NOT NULL,
				PaymentAmount DECIMAL(10,2) NOT NULL,
				StudentID INT NOT NULL,
				CONSTRAINT FK_Payments_Studens
				FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
			 )

CREATE TABLE Subjects
			 (
				SubjectID INT PRIMARY KEY IDENTITY,
				SubjectName NVARCHAR(50) NOT NULL
			 )

CREATE TABLE Agenda
			 (
				StudentID INT NOT NULL,
				SubjectID INT NOT NULL,
				CONSTRAINT PK_Agenda
				PRIMARY KEY(StudentID, SubjectID),
				CONSTRAINT FK_Agenda_Students
				FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
				CONSTRAINT FK_Agenda_Subjects
				FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID)
			 )



