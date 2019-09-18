CREATE DATABASE People

Go

USE People

CREATE TABLE People (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] nvarchar(200) NOT NULL,
	Picture varbinary(max),
	CHECK (DATALENGTH(Picture) <= 2097152),
	Height decimal (3,2),
	[Weight] decimal (5,2),
	Gender char(1) NOT NULL,
	CHECK (Gender = 'f' OR Gender = 'm'),
	Birthdate date NOT NULL,
	Biography nvarchar(max)
)

INSERT INTO People ([Name], Picture, Height, [Weight], Gender, Birthdate, Biography) VALUES
('Ivan Ivanov Ivanov', NULL, 1.99, 109.99, 'm', '1989-01-12', 'redneck number 1'),
('Vasil Vasilev Vasilev', NULL, 1.79, 79.99, 'm', '1988-11-11', 'redneck number 2'),
('Boyka Boykova Boykova', NULL, 1.89, 99.99, 'f', '1960-10-02', 'redneck number 3 BBB'),
('Todor Todorov Todorov', NULL, 1.59, 69.99, 'm', '1979-03-06', 'redneck number 4'),
('Stamat Stamatov Stamatov', NULL, 1.79, 89.09, 'm', '1975-12-27', 'redneck number 5')

SELECT * FROM People
