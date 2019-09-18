CREATE DATABASE Minions

Go

USE Minions

Go

CREATE TABLE Minions (
	Id INT,
	[Name] NVARCHAR(30) NOT NULL,
	Age INT
)

CREATE TABLE Towns (
	Id INT,
	[Name] NVARCHAR(30) NOT NULL
)

Go

--ALTER TABLE Minions
--ALTER COLUMN Id INT NOT NULL

--ALTER TABLE Towns
--ALTER COLUMN Id INT NOT NULL

ALTER TABLE Minions
ADD CONSTRAINT PK_Id
PRIMARY KEY (Id)

ALTER TABLE Towns
ADD CONSTRAINT PK_TownId
PRIMARY KEY (Id)

Go

ALTER TABLE Minions
ADD TownId INT

Go

ALTER TABLE Minions
ADD CONSTRAINT FK_TownIdToId
FOREIGN KEY (TownId) REFERENCES Towns (id)

Go

INSERT INTO Towns (Id, [Name]) VALUES
(1, 'Sofia'),
(2, 'Plovdiv'),
(3, 'Varna')

INSERT INTO Minions (Id, [Name], Age, TownId) VALUES
(1, 'Kevin', 22, 1),
(2, 'Bob', 15, 3),
(3, 'Steward', NULL, 2)
 
 Go

 TRUNCATE TABLE Minions

 Go

 DROP TABLE Minions
