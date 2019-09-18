CREATE DATABASE SoftUni

USE SoftUni

Go

CREATE TABLE Towns (
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL
)

CREATE TABLE Addresses (
	Id INT PRIMARY KEY IDENTITY,
	AddressText NVARCHAR(100) NOT NULL,
	TownId INT NOT NULL
)

CREATE TABLE Departments  (
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL
)

CREATE TABLE Employees  (
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	MiddleName NVARCHAR(50),
	LastName NVARCHAR(50) NOT NULL,
	JobTitle NVARCHAR(100) NOT NULL,
	DepartmentId INT NOT NULL,
	HireDate DATE NOT NULL,
	Salary DECIMAL(7,2) NOT NULL,
	AddressId INT
)

Go

ALTER TABLE Addresses
ADD CONSTRAINT FK_Town
FOREIGN KEY (TownId) REFERENCES Towns(Id)

ALTER TABLE Employees
ADD CONSTRAINT FK_DepartmentId
FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)

ALTER TABLE Employees
ADD CONSTRAINT FK_AddressId
FOREIGN KEY (AddressId) REFERENCES Addresses(Id)

Go

USE master

DROP DATABASE SoftUni

Go

USE SoftUni

INSERT INTO Towns ([Name]) VALUES
('Sofia'),
('Plovdiv'),
('Varna'),
('Burgas')

INSERT INTO Departments([Name]) VALUES
('Engineering'),
('Sales'),
('Marketing'),
('Software Development'),
('Quality Assurance')

INSERT INTO Employees(FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId) VALUES
('Ivan', 'Ivanov', 'Ivanov', '.NET Developer', 4, '2013-02-01', 3500.00, NULL),
('Petar', 'Petrov', 'Petrov', 'Senior Engineer', 1, '2004-03-02', 4000.00, NULL),
('Maria', 'Petrova', 'Ivanova', 'Intern', 5, '2006-08-28', 525.25, NULL),
('Georgi', 'Terziev', 'Ivanov', 'CEO', 2, '2007-12-09', 3000.00, NULL),
('Peter', 'Pan', 'Pan', 'Intern', 3, '2016-08-28', 599.88, NULL)

Go

SELECT * FROM Towns

SELECT * FROM Departments

SELECT * FROM Employees

Go

SELECT * FROM Towns
ORDER BY [Name]

SELECT * FROM Departments
ORDER BY [Name]

SELECT * FROM Employees
ORDER BY Salary DESC

Go

SELECT [Name] FROM Towns
ORDER BY [Name]

SELECT [Name] FROM Departments
ORDER BY [Name]

SELECT FirstName, LastName, JobTitle, Salary FROM Employees
ORDER BY Salary DESC

Go

UPDATE Employees
SET Salary *= 1.1

SELECT Salary FROM Employees

Go
