CREATE DATABASE ColonialJourney 
USE ColonialJourney

GO

--01. DDL
CREATE TABLE Planets
			 (
				Id INT PRIMARY KEY IDENTITY,
				[Name] VARCHAR(30) NOT NULL
			 )

CREATE TABLE Spaceports
			 (
				Id INT PRIMARY KEY IDENTITY,
				[Name] VARCHAR(50) NOT NULL,
				PlanetId INT FOREIGN KEY REFERENCES Planets(Id) NOT NULL
			 )

CREATE TABLE Spaceships
			 (
				Id INT PRIMARY KEY IDENTITY,
				[Name] VARCHAR(50) NOT NULL,
				Manufacturer VARCHAR(30) NOT NULL,
				LightSpeedRate INT DEFAULT 0
			 )

CREATE TABLE Colonists
			 (
				Id INT PRIMARY KEY IDENTITY,
				FirstName VARCHAR(20) NOT NULL,
				LastName VARCHAR(20) NOT NULL,
				Ucn VARCHAR(10) UNIQUE NOT NULL,
				BirthDate DATE NOT NULL
			 )

CREATE TABLE Journeys
			 (
				Id INT PRIMARY KEY IDENTITY,
				JourneyStart DATE NOT NULL,
				JourneyEnd DATE NOT NULL,
				Purpose VARCHAR(11) NOT NULL,
				CHECK (Purpose IN ('Medical', 'Technical', 'Educational', 'Military')),
				DestinationSpaceportId INT FOREIGN KEY REFERENCES Spaceports(Id) NOT NULL,
				SpaceshipId INT FOREIGN KEY REFERENCES Spaceships(Id) NOT NULL
			 )

CREATE TABLE TravelCards
			 (
				Id INT PRIMARY KEY IDENTITY,
				CardNumber CHAR(10) UNIQUE NOT NULL,
				JobDuringJourney VARCHAR(8),
				CHECK (JobDuringJourney IN ('Pilot', 'Engineer', 'Trooper', 'Cleaner', 'Cook')),
				ColonistId INT FOREIGN KEY REFERENCES Colonists(Id) NOT NULL,
				JourneyId INT FOREIGN KEY REFERENCES Journeys(Id) NOT NULL
			 )

GO

--02. Insert
INSERT INTO Planets ([Name]) VALUES
('Mars'),
('Earth'),
('Jupiter'),
('Saturn')

INSERT INTO Spaceships ([Name], Manufacturer, LightSpeedRate) VALUES
('Golf', 'VW', 3),
('WakaWaka', 'Wakanda' ,4),
('Falcon9',	'SpaceX', 1),
('Bed', 'Vidolov', 6)

GO

--03. Update
UPDATE Spaceships
   SET LightSpeedRate += 1
 WHERE Id BETWEEN 8 AND 12

GO

--04. Delete
DELETE FROM TravelCards
WHERE JourneyId IN (1,2,3)

DELETE FROM Journeys
WHERE Id IN (1,2,3)

GO

--05. Select All Travel Cards
  SELECT CardNumber, JobDuringJourney 
    FROM TravelCards
ORDER BY CardNumber

GO

--06. Select All Colonists
  SELECT Id, CONCAT(FirstName, ' ', LastName), Ucn
    FROM Colonists
ORDER BY FirstName, LastName, Id

GO

--07. Select All Military Journeys
  SELECT Id, CONVERT(varchar, JourneyStart, 103) AS JourneyStart, CONVERT(varchar, JourneyEnd, 103) AS JourneyEnd 
    FROM Journeys
   WHERE Purpose = 'Military'
ORDER BY JourneyStart

GO

--08. Select All Pilots
  SELECT c.Id AS id, CONCAT(c.FirstName, ' ', c.LastName) AS full_name 
    FROM Colonists AS c
    JOIN TravelCards AS tc ON tc.ColonistId = c.Id
   WHERE tc.JobDuringJourney = 'pilot'
ORDER BY c.Id

GO

--09. Count Colonists
SELECT COUNT(c.Id) AS count
  FROM Colonists AS c
  JOIN TravelCards AS tc ON tc.ColonistId = c.Id
  JOIN Journeys AS j ON tc.JourneyId = j.Id 
 WHERE j.Purpose = 'Technical'

 GO

 --10. Select The Fastest Spaceship
SELECT TOP(1) ss.[Name] AS SpaceshipName, sp.[Name] AS SpaceportName
         FROM Spaceships AS ss
         JOIN Journeys AS j ON j.SpaceshipId = ss.Id
         JOIN Spaceports AS sp ON j.DestinationSpaceportId = sp.Id
     ORDER BY ss.LightSpeedRate DESC

GO

--11. Select Spaceships With Pilots
  SELECT ss.[Name], ss.Manufacturer 
    FROM Spaceships AS ss
    JOIN Journeys AS j ON j.SpaceshipId = ss.Id
    JOIN TravelCards AS tc ON tc.JourneyId = j.Id
    JOIN Colonists AS c ON tc.ColonistId = c.Id
   WHERE DATEDIFF(YEAR, c.BirthDate, '2019-01-01') < 30 AND tc.JobDuringJourney = 'pilot'
ORDER BY ss.[Name]

GO

--12. Select All Educational Mission
  SELECT p.[Name] AS PlanetName, s.[Name] AS SpaceportName
    FROM Planets AS p
    JOIN Spaceports AS s ON s.PlanetId = p.Id
    JOIN Journeys AS j ON j.DestinationSpaceportId = s.Id
   WHERE j.Purpose = 'Educational'
ORDER BY s.[Name] DESC

GO

--13. Planets And Journeys
  SELECT p.[Name] AS PlanetName, COUNT(j.Id) AS JourneysCount 
    FROM Planets AS p
    JOIN Spaceports AS s ON s.PlanetId = p.Id
    JOIN Journeys AS j ON j.DestinationSpaceportId = p.Id
GROUP BY p.[Name]
ORDER BY JourneysCount DESC, p.[Name]

--14. Extract The Shortest Journey
SELECT TOP(1) j.Id, p.[Name] AS PlanetName, s.[Name] AS SpaceportName, j.Purpose AS JourneyPurpose
         FROM Journeys AS j
         JOIN Spaceports AS s ON j.DestinationSpaceportId = s.Id
         JOIN Planets AS p ON s.PlanetId = p.Id
     ORDER BY DATEDIFF(SECOND,JourneyStart, JourneyEnd)

GO

--15. Select The Less Popular Job ????!!!!!????? no good solution
SELECT TOP(1) j.Id, tc.JobDuringJourney
         FROM Journeys AS j
         JOIN TravelCards AS tc ON tc.JourneyId = j.Id
         JOIN Colonists AS c ON tc.ColonistId = c.Id
     ORDER BY DATEDIFF(SECOND,JourneyStart, JourneyEnd) DESC

GO

--16. Select Special Colonists
SELECT * 
  FROM 
       (SELECT tc.JobDuringJourney, 
			   CONCAT(c.FirstName, ' ', c.LastName) AS FullName, 
			   DENSE_RANK() OVER (PARTITION BY tc.JobDuringJourney ORDER BY c.BirthDate) AS JobRank
          FROM Colonists AS c
          JOIN TravelCards AS tc ON tc.ColonistId = c.Id) AS f
 WHERE f.JobRank = 2

 GO

 --17. Planets and Spaceports
   SELECT p.[Name], COUNT(s.Id) AS [Count] 
     FROM Planets AS p
LEFT JOIN Spaceports AS s ON s.PlanetId = p.Id
 GROUP BY p.[Name]
 ORDER BY [Count] DESC, p.[Name]

 GO

--18. Get Colonists Count
CREATE FUNCTION dbo.udf_GetColonistsCount(@planetName VARCHAR (30))
RETURNS INT
AS
BEGIN
	RETURN (SELECT COUNT(c.Id)
			 FROM Planets AS p
			 JOIN Spaceports AS sp ON sp.PlanetId = p.Id
			 JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id
			 JOIN TravelCards AS tc ON tc.JourneyId = j.Id
			 JOIN Colonists AS c ON tc.ColonistId = c.Id
			 WHERE p.[Name] = @planetName)
END

GO

SELECT dbo.udf_GetColonistsCount('Otroyphus')

GO

--19. Change Journey Purpose
CREATE PROC usp_ChangeJourneyPurpose @journeyId INT , @newPurpose VARCHAR(11)
AS
BEGIN
	DECLARE @idExists BIT = (SELECT COUNT(Id) FROM Journeys WHERE Id = @journeyId)

	IF (@idExists = 0)
	BEGIN
		RAISERROR('The journey does not exist!', 16, 1)
		RETURN
	END

	DECLARE @purpose VARCHAR(11) = (SELECT Purpose FROM Journeys WHERE Id = @journeyId)

	IF(@purpose = @newPurpose)
	BEGIN
		RAISERROR('You cannot change the purpose!', 16,1)
		RETURN
	END

	UPDATE Journeys
	SET Purpose = @newPurpose
	WHERE Id = @journeyId
END

GO

EXEC usp_ChangeJourneyPurpose 1, 'Technical'
SELECT * FROM Journeys
EXEC usp_ChangeJourneyPurpose 2, 'Educational'
EXEC usp_ChangeJourneyPurpose 196, 'Technical'

GO 

--20. Deleted Journeys
CREATE TABLE DeletedJourneys
			 (
				Id INT PRIMARY KEY,
				JourneyStart DATE,
				JourneyEnd DATE,
				Purpose VARCHAR(11),
				DestinationSpaceportId INT,
				SpaceshipId INT
			 )

GO

CREATE TRIGGER tr_JourneysDelete ON Journeys
AFTER DELETE
AS
	INSERT INTO DeletedJourneys 
		 SELECT d.Id, d.JourneyStart, d.JourneyEnd, d.Purpose, d.DestinationSpaceportId, d.SpaceshipId 
		   FROM deleted AS d

GO

DELETE FROM TravelCards
WHERE JourneyId =  1

DELETE FROM Journeys
WHERE Id =  1


