CREATE TABLE Users (
	Id BIGINT PRIMARY KEY IDENTITY,
	Username NVARCHAR(30) UNIQUE NOT NULL,
	[Password] NVARCHAR(26) NOT NULL,
	ProfilePicture VARBINARY(MAX), 
	CHECK (DATALENGTH(ProfilePicture) <= 921600),
	LastLoginTime DATETIME2,
	IsDeleted BIT NOT NULL 
)

INSERT INTO Users (Username, [Password], ProfilePicture, LastLoginTime, IsDeleted) VALUES
('vasko', 'vaseka123', NULL, NULL, 0),
('vasko1', 'vaseka1232', NULL, NULL, 0), 
('vasko2', 'vaseka1232', NULL, NULL, 0), 
('vasko3', 'vaseka1233', NULL, NULL, 1), 
('vasko4', 'vaseka1234', NULL, NULL, 1)  

SELECT * FROM Users

ALTER TABLE Users
DROP CONSTRAINT PK__Users__3214EC072429B1D6

Go

ALTER TABLE Users
ADD CONSTRAINT PK_IdAndUsername
PRIMARY KEY (Id, Username)

Go

ALTER TABLE Users
ADD CONSTRAINT Check_Password
CHECK (LEN([Password]) >= 5)

Go 

-- test password constraint
--INSERT INTO Users (Username, [Password], ProfilePicture, LastLoginTime, IsDeleted) VALUES
--('vaskoto', 'vase', NULL, NULL, 0)

ALTER TABLE Users
ADD CONSTRAINT DF_LastLoginTime
DEFAULT GETDATE() FOR LastLoginTime 

--test dafault date
--INSERT INTO Users (Username, [Password], ProfilePicture, IsDeleted) VALUES
--('vaskototo', 'vaseka123', NULL, 0)

ALTER TABLE Users
DROP CONSTRAINT PK_IdAndUsername

ALTER TABLE Users
ADD CONSTRAINT PK_Id
PRIMARY KEY(Id)

ALTER TABLE Users
ADD CONSTRAINT Chek_UsernameLen
CHECK (LEN(Username) >= 3)

