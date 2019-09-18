CREATE DATABASE Movies

USE Movies

CREATE TABLE Directors (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	DirectorName NVARCHAR(100) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Genres (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	GenreName NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Categories (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	CategoryName NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Movies (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	Title NVARCHAR(30) NOT NULL,
	DirectorId INT NOT NULL,
	CopyrightYear SMALLINT NOT NULL,
	CHECK (CopyrightYear >= 1900 AND CopyrightYear <= 2020),
	[Length] TIME NOT NULL,
	GenreId INT NOT NULL,
	CategoryId INT NOT NULL,
	Rating SMALLINT,	
	Notes NVARCHAR(MAX)
)

ALTER TABLE Movies
ADD CONSTRAINT FK_Director
FOREIGN KEY (DirectorId) REFERENCES Directors (Id)

ALTER TABLE Movies
ADD CONSTRAINT FK_Genre
FOREIGN KEY (GenreId) REFERENCES Genres (Id)

ALTER TABLE Movies
ADD CONSTRAINT FK_Category
FOREIGN KEY (CategoryId) REFERENCES Categories (Id)

ALTER TABLE Movies
ADD CONSTRAINT CH_Rating
CHECK (Rating >= 0 AND Rating <= 10)

INSERT INTO Directors (DirectorName, Notes) VALUES
('Magardich Halvadzhian', 'The best Armenian movie ever'),
('Lyubomir Neykov', 'The best comedy movie ever'),
('Evtim Miloshev', 'The best horor movie ever'),
('Slavi Trifonov', NULL),
('Metodi Manchenko', 'The best sport movie ever')

INSERT INTO Genres (GenreName, Notes) VALUES
('Sy-Fi', 'some note'),
('Comedy', 'another note'),
('Historic', 'third note'),
('Biography', NULL),
('Horor', 'scary note')

INSERT INTO Categories(CategoryName, Notes) VALUES
('Adults', 'secret notes'),
('Kids', 'simply notes'),
('Infants', NULL),
('Elder', 'BIG LETTER NOTE'),
('All age', 'note')

INSERT INTO Movies(Title, DirectorId, CopyrightYear, [Length], GenreId, CategoryId, Rating, Notes) VALUES
('Откраднат Живот', 5, 2015, '01:10:20', 2, 3, 10, 'Good one'),
('Batman', 2, 2005, '01:40:00', 1, 4, 8, 'old ... verry old'),
('The Lord of The Rings', 3, 2010, '02:40:30', 5, 1, 9, 'Long ... too long'),
('Bad Boys 1', 1, 2008, '02:20:20', 5, 5, 10, 'Classy'),
('Man in Black 1', 4, 2006, '01:50:20', 4, 2, 7, NULL)