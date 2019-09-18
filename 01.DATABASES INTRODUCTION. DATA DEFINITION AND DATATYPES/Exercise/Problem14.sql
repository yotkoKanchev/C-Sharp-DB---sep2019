CREATE DATABASE CarRental

Go

Use CarRental

CREATE TABLE Categories  (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	CategoryName NVARCHAR(30) NOT NULL,
	DailyRate DECIMAL(6,2) NOT NULL,
	WeeklyRate DECIMAL (8,2) NOT NULL,
	MonthlyRate DECIMAL (9,2) NOT NULL,
	WeekendRate DECIMAL (6,2) NOT NULL
)

CREATE TABLE Cars (
	ID INT PRIMARY KEY IDENTITY NOT NULL,
	PlateNumber NVARCHAR(20) NOT NULL,
	Manufacturer NVARCHAR (100) NOT NULL,
	Model VARCHAR(30) NOT NULL,
	CarYear SMALLINT NOT NULL,
	CHECK (CarYear >= 1900 AND CarYear <= 2020),
	CategoryId INT NOT NULL,
	Doors SMALLINT NOT NULL,
	Picture VARBINARY(MAX),
	Condition BIT NOT NULL,
	Available BIT NOT NULL
)

CREATE TABLE Employees (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	Title NVARCHAR(20) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Customers (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	DriverLicenceNumber BIGINT NOT NULL,
	FullName NVARCHAR(100) NOT NULL,
	[Address] NVARCHAR(100) NOT NULL,
	City NVARCHAR(100) NOT NULL,
	ZIPCode NVARCHAR(20),
	Notes NVARCHAR(MAX)
)

CREATE TABLE RentalOrders (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT NOT NULL,
	CustomerId INT NOT NULL,
	CarId INT NOT NULL,
	TankLevel SMALLINT NOT NULL,
	CHECK (TankLevel >= 0 AND TankLevel <= 10),
	KilometrageStart INT NOT NULL,
	CHECK (KilometrageStart >= 0),
	KilometrageEnd INT NOT NULL,
	CHECK( KilometrageEnd >= KilometrageStart),
	TotalKilometrage INT NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays INT NOT NULL,
	CHECK (TotalDays >= 0),
	RateApplied DECIMAL(8,2) NOT NULL,
	TaxRate DECIMAL (8,2) NOT NULL,
	OrderStatus BIT NOT NULL,
	Notes NVARCHAR(MAX)
)

Go

ALTER TABLE RentalOrders
ADD CONSTRAINT FK_EmployeeId
FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)

ALTER TABLE RentalOrders
ADD CONSTRAINT FK_CustomerId
FOREIGN KEY (CustomerId) REFERENCES Customers(Id)

ALTER TABLE RentalOrders
ADD CONSTRAINT FK_CarId
FOREIGN KEY (CarId) REFERENCES Cars(Id)

ALTER TABLE Cars
ADD CONSTRAINT FK_CategoryId
FOREIGN KEY (CategoryId) REFERENCES Categories(Id)

Go

INSERT INTO Categories (CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate) VALUES
('Economy', 30, 150, 500, 80),
('MidClass', 50, 250, 1000, 130),
('HighClass', 100, 500, 2000, 250)

INSERT INTO Cars (PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Available) VALUES
('CA9999AC', 'BMW', '740i', 2019, 3, 4, NULL, 0, 1),
('CB1111BC', 'Mercedes', 'S500', 2017, 1, 4, NULL, 1, 1),
('CB555555', 'Honda', 'Accord', 2018, 2, 5, NULL, 1, 1)

INSERT INTO Employees (FirstName, LastName, Title, Notes ) VALUES
('Vasil', 'Boyanov', 'Mr.', 'Bay mangal'),
('Boyko', 'Borisov', 'Mr.', 'BB'),
('Slavi', 'Trifonov', 'Mr.', NULL)

INSERT INTO Customers (DriverLicenceNumber, FullName, [Address], City, ZIPCode, Notes) VALUES
(999000123, 'Stamat I Stamatov', '1 Vasil Levski Str.', 'Tranchovitsa', '3333', 'Smart guy'),
(999000124, 'Georgi Zhorov Goshov', '1 Hristo Botev Str.', 'Dupnitsa', '2400', 'Dummy'),
(999000125, 'Ivan Vankov Bayivanov', '2 Izgrev Str.', 'Kosinbrod', '1400', 'Compleate Redneck')

INSERT INTO RentalOrders (EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, 
						  StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes) VALUES
(1, 2, 3, 10, 26000, 29000, 3000, '2019-01-01', '2019-01-31', 31, 100, 12.60, 1, NULL),
(2, 1, 1, 9, 33000, 35000, 2000, '2019-02-01', '2019-02-21', 21, 30, 5.40, 1, NULL),
(3, 3, 2, 4, 84000, 85000, 1000, '2019-03-10', '2019-03-15', 6, 80, 8.30, 0, NULL)
