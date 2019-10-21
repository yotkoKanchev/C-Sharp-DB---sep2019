CREATE DATABASE Airport
USE Airport

GO

--01. DDL
CREATE TABLE Planes
			 (
				Id INT PRIMARY KEY IDENTITY,
				[Name] NVARCHAR(30) NOT NULL,
				Seats INT NOT NULL,
				[Range] INT NOT NULL
			 )

CREATE TABLE Flights
			 (
				Id INT PRIMARY KEY IDENTITY,
				DepartureTime DATETIME,
				ArrivalTime DATETIME,
				Origin NVARCHAR(50) NOT NULL,
				Destination NVARCHAR(50) NOT NULL,
				PlaneId INT FOREIGN KEY REFERENCES Planes(Id) NOT NULL
			 )

CREATE TABLE LuggageTypes
			 (
				Id INT PRIMARY KEY IDENTITY,
				[Type] NVARCHAR(30) NOT NULL
			 )

CREATE TABLE Passengers
			 (
				Id INT PRIMARY KEY IDENTITY,
				FirstName NVARCHAR(30) NOT NULL,
				LastName NVARCHAR(30) NOT NULL,
				Age INT NOT NULL,
				[Address] NVARCHAR(30) NOT NULL,
				PassportId CHAR(11) NOT NULL
			 )

CREATE TABLE Luggages
			 (
				Id INT PRIMARY KEY IDENTITY,
				LuggageTypeId INT FOREIGN KEY REFERENCES LuggageTypes(Id) NOT NULL,
				PassengerId INT FOREIGN KEY REFERENCES Passengers(Id) NOT NULL
			 )

CREATE TABLE Tickets
			 (
				Id INT PRIMARY KEY IDENTITY,
				PassengerId INT FOREIGN KEY REFERENCES Passengers(Id) NOT NULL,
				FlightId INT FOREIGN KEY REFERENCES Flights(Id) NOT NULL,
				LuggageId INT FOREIGN KEY REFERENCES Luggages(Id) NOT NULL,
				Price DECIMAL(10,2) NOT NULL
			 )

GO

--02. Insert
INSERT INTO Planes 
	 VALUES ('Airbus 336', 112, 5132),
            ('Airbus 330', 432, 5325),
            ('Boeing 369', 231, 2355),
            ('Stelt 297', 254, 2143),
            ('Boeing 338', 165, 5111),
            ('Airbus 558', 387, 1342),
            ('Boeing 128', 345, 5541)

INSERT INTO LuggageTypes
	 VALUES ('Crossbody Bag'),
			('School Backpack'),
			('Shoulder Bag')

GO

--03. Update
UPDATE Tickets 
SET Price *= 1.13
WHERE FlightId = (SELECT Id FROM Flights WHERE Destination = 'Carlsbad') 

GO

--04. Delete
DELETE FROM Tickets 
WHERE FlightId = (SELECT Id FROM Flights WHERE Destination = 'Ayn Halagim') 

DELETE FROM Flights
WHERE Destination = 'Ayn Halagim'

GO

--05. The "Tr" Planes
SELECT * FROM Planes
WHERE [Name] LIKE '%tr%'
ORDER BY Id, [Name], Seats, [Range]

GO

--07. Flight Profits
  SELECT FlightId, SUM(Price) AS Price
    FROM Tickets
GROUP BY FlightId
ORDER BY SUM(Price) DESC, FlightId

GO

--7. Passanger Trips
  SELECT CONCAT(p.FirstName, ' ', p.LastName) AS [Full Name],
  	     f.Origin, f.Destination
    FROM Passengers AS p
    JOIN Tickets AS t ON t.PassengerId = p.Id
    JOIN Flights AS f ON f.Id = t.FlightId
ORDER BY CONCAT(p.FirstName, ' ', p.LastName), f.Origin, f.Destination 

GO

--8. Non Adventures People
   SELECT p.FirstName AS [First Name], p.LastName AS [Last Name], p.Age
     FROM Passengers AS p
LEFT JOIN Tickets AS T ON t.PassengerId = p.Id
    WHERE t.PassengerId IS NULL 
 ORDER BY p.Age DESC, p.FirstName, p.LastName

 --9. Full Info
  SELECT CONCAT(p.FirstName, ' ', p.LastName) AS [Full Name], 
		 pl.[Name] AS [Plane Name],
		 CONCAT(f.Origin, ' - ', f.Destination) AS Trip,
		 lt.[Type] AS [Luggage Type]
    FROM Passengers AS p
	JOIN Tickets AS t ON t.PassengerId = p.Id
	JOIN Flights AS f ON f.Id = t.FlightId
	JOIN Planes AS pl ON pl.Id = f.PlaneId
    JOIN Luggages AS l ON l.Id = t.LuggageId
	JOIN LuggageTypes AS lt ON lt.Id = l.LuggageTypeId
ORDER BY [Full Name], pl.[Name], f.Origin, f.DepartureTime, lt.[Type]

--10. PSP
  SELECT p.[Name], p.Seats, 
		 COUNT(t.PassengerId) AS [Passengers Count]
    FROM Planes AS p
    LEFT JOIN Flights AS f ON f.PlaneId = p.Id
    LEFT JOIN Tickets AS t ON t.FlightId = f.Id
GROUP BY p.[Name], p.Seats
ORDER BY COUNT(t.PassengerId) DESC, p.[Name], p.Seats

--11. Vacation
CREATE FUNCTION udf_CalculateTickets(@origin VARCHAR(50), @destination VARCHAR(50), @peopleCount INT) 
RETURNS VARCHAR(100)
AS
BEGIN
	IF(@peopleCount <= 0)
	BEGIN
		RETURN 'Invalid people count!'
	END

	DECLARE @flightID INT = (SELECT TOP(1) Id FROM Flights WHERE Origin = @origin AND Destination = @destination) 
	IF(@flightID IS NULL)
	BEGIN
		RETURN 'Invalid flight!'
	END

	DECLARE @price DECIMAL(24,2) = (SELECT TOP(1) t.Price 
						 	 FROM Tickets AS t 
						 	 JOIN Flights AS f ON f.Id = t.FlightId
						    WHERE f.Origin = @origin AND f.Destination = @destination)

	RETURN 'Total price ' + CAST(@peopleCount * @price AS VARCHAR(20))
END

GO

--12. Wrong Data
CREATE PROC usp_CancelFlights
AS
UPDATE Flights
SET DepartureTime = NULL, ArrivalTime = NULL
WHERE DATEDIFF(SECOND, DepartureTime, ArrivalTime) > 0

GO

