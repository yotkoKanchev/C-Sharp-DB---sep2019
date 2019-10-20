CREATE DATABASE TripService
USE TripService

GO

--01. DDL
CREATE TABLE Cities
	(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(20) NOT NULL,
	CountryCode CHAR(2) NOT NULL
	)

CREATE TABLE Hotels
	(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	EmployeeCount INT NOT NULL,
	BaseRate DECIMAL(18,2) NOT NULL
	)

CREATE TABLE Rooms
	(
	Id INT PRIMARY KEY IDENTITY,
	Price DECIMAL(18,2) NOT NULL,
	[Type] NVARCHAR(20) NOT NULL,
	Beds INT NOT NULL,
	HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
	)

CREATE TABLE Trips
	(
	Id INT PRIMARY KEY IDENTITY,
	RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL,
	BookDate DATE NOT NULL,
	ArrivalDate DATE NOT NULL,
	CHECK (BookDate < ArrivalDate),
	ReturnDate DATE NOT NULL,
	CHECK (ArrivalDate < ReturnDate),
	CancelDate DATE
	)

CREATE TABLE Accounts
	(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	MiddleName NVARCHAR(20),
	LastName NVARCHAR(50) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	BirthDate DATE NOT NULL,
	Email VARCHAR(100) UNIQUE NOT NULL
	)

CREATE TABLE AccountsTrips
	(
	AccountId INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL,
	TripId INT FOREIGN KEY REFERENCES Trips(Id) NOT NULL,
	Luggage INT NOT NULL,
	CHECK (Luggage >= 0),
	CONSTRAINT PK_AccountsTrips
	PRIMARY KEY (AccountId, TripId)
	)

GO

--02. Insert
INSERT INTO Accounts (FirstName, MiddleName, LastName, CityId, BirthDate, Email) VALUES
('John', 'Smith', 'Smith', 34,	'1975-07-21', 'j_smith@gmail.com'),
('Gosho', NULL,	'Petrov', 11, '1978-05-16',	'g_petrov@gmail.com'),
('Ivan', 'Petrovich', 'Pavlov', 59, '1849-09-26', 'i_pavlov@softuni.bg'),
('Friedrich', 'Wilhelm', 'Nietzsche', 2, '1844-10-15', 'f_nietzsche@softuni.bg')

INSERT INTO Trips (RoomId, BookDate, ArrivalDate, ReturnDate, CancelDate) VALUES
(101, '2015-04-12',	'2015-04-14', '2015-04-20', '2015-02-02'),
(102, '2015-07-07',	'2015-07-15', '2015-07-22', '2015-04-29'),
(103, '2013-07-17',	'2013-07-23', '2013-07-24', NULL),
(104, '2012-03-17',	'2012-03-31', '2012-04-01', '2012-01-10'),
(109, '2017-08-07',	'2017-08-28', '2017-08-29', NULL)

GO

--03. Update
UPDATE Rooms
SET Price *= 1.14
WHERE HotelId IN (5,7,9)

GO

--04. Delete
DELETE FROM AccountsTrips
WHERE AccountId = 47

GO

--05. Bulgarian Cities
SELECT Id, [Name]
  FROM Cities
 WHERE CountryCode = 'bg'
ORDER BY [Name]

GO

--06. People Born After 1991
  SELECT CONCAT(FirstName, ' ', MiddleName + ' ', LastName) AS [Full Name],
  	     DATEPART(YEAR, BirthDate) AS BirthYear
    FROM Accounts
   WHERE DATEPART(YEAR, BirthDate) > 1991
ORDER BY BirthYear DESC, FirstName

GO

--07. EEE-Mails
  SELECT a.FirstName, a.LastName,FORMAT(a.BirthDate, 'MM-dd-yyyy') AS BirthDate, c.[Name] AS Hometown, a.Email 
    FROM Accounts AS a
    JOIN Cities AS c ON a.CityId = c.Id
   WHERE LEFT(a.Email, 1) = 'e' -- a.Email LIKE 'e%'
ORDER BY c.[Name] DESC

GO

--08. City Statistics
   SELECT c.[Name], COUNT(h.Id) AS Hotels
     FROM Cities AS c
LEFT JOIN Hotels AS h ON h.CityId = c.Id
 GROUP BY c.[Name]
 ORDER BY Hotels DESC, c.[Name]

GO

--09. Expensive First Class Rooms
  SELECT r.Id, r.Price, h.[Name] AS Hotel, c.[Name] AS City
    FROM Rooms AS r
    JOIN Hotels AS h ON r.HotelId = h.id
    JOIN Cities AS c ON h.CityId = c.Id
   WHERE r.[Type] = 'First Class'
ORDER BY r.Price DESC, r.Id

GO

--10. Longest and Shortest Trips
SELECT a.Id AS AccountId, 
	   CONCAT(a.FirstName, ' ', a.LastName) AS [FullName],
	   MAX(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate)) AS LongestTrip,
	   MIN(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate)) AS ShortestTrip
  FROM Accounts AS a
  JOIN AccountsTrips AS at ON at.AccountId = a.Id
  JOIN Trips AS t ON at.TripId = t.Id
  WHERE a.MiddleName IS NULL AND t.CancelDate IS NULL
GROUP BY a.FirstName, a.LastName, a.Id
ORDER BY [LongestTrip] DESC, a.Id
  
GO

--11. Metropolis
SELECT TOP(5)
		   c.Id, c.[Name] AS City, 
		   c.CountryCode AS Country, 
		   COUNT(a.Id) AS Accounts
      FROM Cities AS c
      JOIN Accounts AS a ON a.CityId = c.Id
  GROUP BY c.[Name], c.CountryCode, c.Id
  ORDER BY Accounts DESC

GO

--12. Romantic Getaways
  SELECT a.Id, a.Email, c.[Name] AS City, COUNT(c.Id) AS Trips
    FROM Accounts AS a
    JOIN AccountsTrips AS at ON at.AccountId = a.Id
    JOIN Cities AS c ON a.CityId = c.Id
    JOIN Hotels AS h ON h.CityId = c.Id
    JOIN Trips AS t ON at.TripId = t.Id
    JOIN Rooms AS r ON t.RoomId = r.Id
   WHERE r.HotelId  = h.Id
GROUP BY a.Id, a.Email, c.[Name]
ORDER BY Trips DESC, a.Id

GO

--13. Lucrative Destinations
SELECT TOP(10)
		   c.Id, c.[Name], 
		   SUM(h.BaseRate + r.Price) AS [Total Revenue], 
		   COUNT(t.Id) AS Trips
      FROM Cities AS c
      JOIN Hotels AS h ON h.CityId = c.Id
      JOIN Rooms AS r ON r.HotelId = h.Id
      JOIN Trips AS t ON t.RoomId = r.Id
	 WHERE DATEPART(YEAR, t.BookDate) = 2016
  GROUP BY c.Id, c.[Name]
  ORDER BY [Total Revenue] DESC, Trips DESC

GO

--14. Trip Revenues
SELECT at.TripId, h.[Name] AS HotelName, r.[Type] AS RoomType,
  IIF(t.CancelDate IS NOT NULL, 0.0 , SUM(h.BaseRate + r.Price)) AS Revenue
  FROM Trips AS t
  JOIN Rooms AS r ON t.RoomId = r.Id
  JOIN Hotels AS h ON r.HotelId = h.id
  JOIN AccountsTrips AS at ON at.TripId = t.Id
  GROUP BY at.TripId, r.[Type], h.[Name] , t.CancelDate
  ORDER BY r.[Type], at.TripId	

GO

--15. Top Travelers
  SELECT ranked.Id, ranked.Email, ranked.CountryCode, ranked.Trips
    FROM (
  		    SELECT a.Id, a.Email, c.CountryCode , 
  		  	       COUNT(t.Id) AS Trips,
  		  	       ROW_NUMBER() OVER (PARTITION BY c.CountryCode ORDER BY COUNT(t.Id) DESC) AS [Rank]
              FROM Accounts AS a 
              JOIN AccountsTrips AS at ON at.AccountId = a.Id
              JOIN Trips AS t ON at.TripId = t.Id
              JOIN Rooms AS r ON t.RoomId = r.Id
              JOIN Hotels AS h ON r.HotelId = h.Id
              JOIN Cities AS c ON h.CityId = c.Id
          GROUP BY a.Id, a.Email, c.CountryCode) AS ranked
   WHERE ranked.Rank = 1
ORDER BY Trips DESC, ranked.Id

GO

--16. Luggage Fees
   SELECT t.Id, 
		  SUM(at.Luggage) AS Luggage, 
          CONCAT('$', 5 * IIF(SUM(at.Luggage) > 5,  SUM(at.Luggage), 0)) AS Fee
     FROM Trips AS t
LEFT JOIN AccountsTrips AS at ON at.TripId = t.Id
    WHERE at.Luggage > 0
 GROUP BY t.Id
 ORDER BY Luggage DESC

GO

--17. GDPR Violation
SELECT t.Id, CONCAT(a.FirstName, ' ', a.MiddleName + ' ', a.LastName) AS [Full Name],
	   c.[Name] AS [From],
	   ch.[Name] AS [To],
	   IIF(t.CancelDate IS NULL, CAST(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate) AS VARCHAR(max)) + ' days', 'Canceled') AS [Duration]
  FROM Accounts AS a 
  JOIN Cities AS c ON a.CityId = c.Id
  JOIN AccountsTrips AS at ON at.AccountId = a.Id
  JOIN Trips AS t ON at.TripId = t.Id
  JOIN Rooms AS r ON t.RoomId = r.Id
  JOIN Hotels AS h ON r.HotelId = h.Id
  JOIN Cities AS ch ON h.CityId = ch.Id
--GROUP BY t.Id, a.FirstName, a.MiddleName, a.LastName
ORDER BY [Full Name], t.Id

GO

--18. Available Room 5/7 !!!!
CREATE FUNCTION udf_GetAvailableRoom(@hotelId INT, @date DATE, @people INT)
RETURNS NVARCHAR(max)
AS
BEGIN
	DECLARE @roomIsAvailable BIT = (
	SELECT TOP(1) COUNT(r.Id) 
	  FROM Hotels AS h
	  JOIN Rooms AS r ON r.HotelId = h.Id
	  JOIN Trips AS t ON t.RoomId = r.Id
	  WHERE h.Id = @hotelId AND r.Beds >= @people AND (@date NOT BETWEEN t.ArrivalDate AND t.ReturnDate OR (@date BETWEEN t.ArrivalDate AND t.ReturnDate AND t.CancelDate IS NOT NULL))
	)

	IF(@roomIsAvailable = 0)
	BEGIN
		RETURN 'No rooms available'
	END

	DECLARE @choosenRoom INT = (
	SELECT TOP(1) r.Id
	  FROM Hotels AS h
	  JOIN Rooms AS r ON r.HotelId = h.Id
	  JOIN Trips AS t ON t.RoomId = r.Id
	  WHERE h.Id = @hotelId AND r.Beds >= @people AND r.Id IN (SELECT r.Id FROM Rooms AS r JOIN Hotels AS s ON r.HotelId = s.Id JOIN Trips AS t ON t.RoomId = r.Id WHERE @date NOT BETWEEN t.ArrivalDate AND t.ReturnDate AND t.CancelDate IS NULL)
	  ORDER BY r.Price DESC
	)
	DECLARE @roomType NVARCHAR(10) = (SELECT [Type] FROM Rooms WHERE Id = @choosenRoom)
	DECLARE @roomBedsCount INT = (SELECT Beds FROM Rooms WHERE Id = @choosenRoom)
	DECLARE @totalPrice DECIMAL(18,2) = (SELECT h.BaseRate + r.Price FROM Rooms AS r JOIN Hotels AS h ON r.HotelId = h.Id WHERE h.Id = @hotelId AND r.Id = @choosenRoom) * @people

	RETURN CONCAT('Room ',@choosenRoom, ': ', @roomType, 's (' ,@roomBedsCount, ' beds) - $',@totalPrice)
END 

GO

--19. Switch Room 4/7 !!!
CREATE PROC usp_SwitchRoom @tripId INT , @targetRoomId INT
AS
BEGIN
	DECLARE @tripHotelId INT = (SELECT h.Id FROM Trips AS t JOIN Rooms AS r ON t.RoomId = r.Id JOIN Hotels AS h ON r.HotelId = h.Id WHERE t.Id = @tripId)
	DECLARE @roomHotelId INT = (SELECT h.Id FROM Rooms AS r JOIN Hotels AS h ON r.HotelId = h.Id WHERE r.Id = @targetRoomId)

	IF (@tripHotelId <> @roomHotelId)
	BEGIN 
		RAISERROR ('Target room is in another hotel!', 16, 1)
		RETURN
	END

	DECLARE @roomBedsCount INT = (SELECT Beds FROM Rooms AS r JOIN Hotels AS h ON r.HotelId = h.Id WHERE h.Id = @roomHotelId AND r.Id = @targetRoomId)
	DECLARE @tripBedsCount INT = (SELECT COUNT(*) FROM Rooms AS r JOIN Trips AS t ON t.RoomId = r.Id JOIN Hotels AS h ON r.HotelId = h.Id WHERE h.Id = @tripHotelId AND t.Id = @tripId)

	IF(@roomBedsCount < @tripBedsCount)
	BEGIN 
		RAISERROR ('Not enough beds in target room!', 16, 1)
		RETURN
	END

	UPDATE Trips
	SET RoomId = @targetRoomId
	WHERE Id = @tripId
END

EXEC usp_SwitchRoom 10, 8

GO

--20. Cancel Trip
CREATE TRIGGER tr_TripDeleted ON Trips
INSTEAD OF DELETE
AS
UPDATE Trips
SET CancelDate = GETDATE()
WHERE Id IN (SELECT Id FROM deleted) AND CancelDate IS NULL
