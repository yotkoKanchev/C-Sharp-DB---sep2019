CREATE DATABASE Hotel

USE Hotel

CREATE TABLE Employees (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	Title NVARCHAR(20),
	Notes NVARCHAR(MAX)
)

CREATE TABLE Customers  (
	AccountNumber INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	PhoneNumber BIGINT,
	EmergencyName NVARCHAR(100),
	EmergencyNumber BIGINT,
	Notes NVARCHAR(MAX)
)

CREATE TABLE RoomStatus (
	RoomStatus NVARCHAR(30) PRIMARY KEY NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE RoomTypes (
	RoomType NVARCHAR(30) PRIMARY KEY NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE BedTypes  (
	BedType NVARCHAR(30) PRIMARY KEY NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Rooms (
	RoomNumber INT PRIMARY KEY NOT NULL,
	RoomType NVARCHAR(30) NOT NULL,
	BedType NVARCHAR(30) NOT NULL,
	Rate DECIMAL (7,2) NOT NULL,
	RoomStatus NVARCHAR(30) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Payments (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT NOT NULL,
	PaymentDate DATE NOT NULL,
	AccountNumber INT NOT NULL,
	FirstDateOccupied DATE NOT NULL,
	LastDateOccupied DATE NOT NULL,
	CHECK (LastDateOccupied >= FirstDateOccupied),
	TotalDays INT NOT NULL,
	CHECK (TotalDays >= 0),
	AmountCharged DECIMAL(10,2),
	TaxRate DECIMAL(5,2),
	TaxAmount DECIMAL(10,2),
	PaymentTotal DECIMAL(10,2),
	Notes NVARCHAR(MAX)
)

CREATE TABLE Occupancies (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT NOT NULL,
	DateOccupied DATE NOT NULL,
	AccountNumber INT NOT NULL,
	RoomNumber INT NOT NULL,
	RateApplied DECIMAL(7,2) NOT NULL,
	PhoneCharge DECIMAL(10,2),
	Notes NVARCHAR(MAX)
)

ALTER TABLE Rooms
ADD CONSTRAINT FK_RoomType
FOREIGN KEY (RoomType) REFERENCES RoomTypes(RoomType)

ALTER TABLE Rooms
ADD CONSTRAINT FK_BedType
FOREIGN KEY (BedType) REFERENCES BedTypes(BedType)

ALTER TABLE Rooms
ADD CONSTRAINT FK_RoomStatus
FOREIGN KEY (RoomStatus) REFERENCES RoomStatus(RoomStatus)

ALTER TABLE Payments
ADD CONSTRAINT FK_EmployeeId
FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)

ALTER TABLE Occupancies
ADD CONSTRAINT FK_EmployeeId_Occ
FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)

ALTER TABLE Occupancies
ADD CONSTRAINT FK_AccountNumber
FOREIGN KEY (AccountNumber) REFERENCES Customers(AccountNumber)

ALTER TABLE Occupancies
ADD CONSTRAINT FK_RoomNumber
FOREIGN KEY (RoomNumber) REFERENCES Rooms(RoomNumber)

INSERT INTO Employees (FirstName, LastName, Title, Notes) VALUES
('Ivan', 'Todorov', 'Mr.', 'Looking creepy guy'),
('Vasil', 'Boyanov', 'n/a', 'Compleate gipsy'),
('Boyko', 'Borisov', 'Mr.', NULL)

INSERT INTO Customers (FirstName, LastName, PhoneNumber, EmergencyName, EmergencyNumber, Notes) VALUES
('Client1', 'Client1 Family', 00359887887887, 'his mother', 00359888111222, 'Looking strange'),
('Client2', 'Client3 Family', NULL, NULL, NULL, NULL),
('Client2', 'Client3 Family', NULL, 'his father', 00359888444222, NULL)

INSERT INTO RoomStatus (RoomStatus, Notes) VALUES
('empty', 'big room'),
('occuped', 'big room'),
('closed' , 'need repainting')

INSERT INTO RoomTypes(RoomType, Notes) VALUES
('Double', 'standart'),
('Tween', NULL),
('Family', NULL)

INSERT INTO BedTypes(BedType, Notes) VALUES
('Single', 'realy small'),
('King', 'big'),
('Queen', 'biggest one')

INSERT INTO Rooms (RoomNumber, RoomType, BedType, Rate, RoomStatus, Notes) VALUES
(101, 'Double', 'King', 100.99, 'occuped', 'some nonte'),
(201, 'Tween', 'Single', 50.99, 'occuped', 'some nonte'),
(301, 'Family', 'Queen', 120.99, 'occuped', 'some nonte')

INSERT INTO Payments(EmployeeId, PaymentDate, AccountNumber, FirstDateOccupied, LastDateOccupied, TotalDays, 
					AmountCharged, TaxRate, TaxAmount, PaymentTotal, Notes) VALUES
(1, '2019-09-01', 1000000001, '2019-01-01', '2019-08-31', 306, 1004.20, 10, 400.20, 20502.55, NULL),
(2, '2019-09-01', 1000000002, '2019-02-01', '2019-08-31', 276, 804.20, 10, 200.20, 17502.55, NULL),
(3, '2019-09-01', 1000000003, '2019-03-01', '2019-08-31', 246, 704.20, 10, 100.20, 15502.55, NULL)

INSERT INTO Occupancies (EmployeeId, DateOccupied, AccountNumber, RoomNumber, RateApplied, PhoneCharge, Notes) VALUES
(1, '2019-01-01', 1, 101, 100.99, NULL, NULL),
(3, '2019-02-01', 3, 301, 90.99, NULL, NULL),
(2, '2019-03-01', 2, 201, 70.99, NULL, NULL)