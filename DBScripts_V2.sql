--CREATE DATABASE WasherDB
--GO

CREATE DATABASE WasherDB
GO

USE [WasherDB]
GO

/****** Object: Table [dbo].[USERS] Script Date: 28-03-2019 8.29.42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[USERS] (
    [USERID]       VARCHAR (20)  NOT NULL PRIMARY KEY,
    [USERNAME]     VARCHAR (50)  NOT NULL,
    [USEREMAIL]    VARCHAR (100) NOT NULL,
    [USERMOBILE]   VARCHAR (10)  NOT NULL,
    [LATITUDE]     VARCHAR (20)  NOT NULL,
    [LONGITUDE]    VARCHAR (20)  NOT NULL,
    [USERPASSWORD] BINARY (64)   NOT NULL,
    [WASHING]      BIT           NOT NULL
);


USE [WasherDB]
GO

/****** Object: SqlProcedure [dbo].[usp_SignUp] Script Date: 28-03-2019 8.30.16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE usp_SignUp
(
@UserName VARCHAR(50),
@UserEmail VARCHAR(100),
@UserMobile VARCHAR(10),
@lat VARCHAR(20),
@lon VARCHAR(20),
@UserPassword VARCHAR(40),
@Washing BIT,
@UserId VARCHAR(20) OUT
)
AS
BEGIN
 DECLARE @LENGTH INT 
 BEGIN TRY
 IF EXISTS (SELECT USERID FROM USERS WHERE USEREMAIL=@UserEmail)
 BEGIN
 SET @UserId='0'
 RETURN -1
 END
 SELECT @LENGTH=LEN(MAX(USERID))-1 FROM USERS
 PRINT(@LENGTH)
 SELECT @UserId='U'+ 
 CAST(CAST(SUBSTRING(MAX(USERID),2,@LENGTH) AS INT)+1 AS CHAR) 
 FROM USERS
 INSERT INTO USERS VALUES(@UserId,@UserName,@UserEmail,@UserMobile,@lat,@lon,HASHBYTES('SHA2_512', @UserPassword),@Washing)
 RETURN 1
 END TRY
 BEGIN CATCH
 SET @UserId='-1'
 RETURN -99
 END CATCH
END
GO

USE [WasherDB]
GO

/****** Object: Scalar Function [dbo].[UFN_LOGIN] Script Date: 28-03-2019 8.30.25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION UFN_LOGIN(@UserEmail VARCHAR(100),@Password VARCHAR(20))
RETURNS VARCHAR(20)
AS
BEGIN
 DECLARE @USER_ID VARCHAR(20)
 IF NOT EXISTS (SELECT USERID FROM USERS WHERE USEREMAIL=@UserEmail AND USERPASSWORD=HASHBYTES('SHA2_512',@Password))
 RETURN 'NA'
 SELECT @USER_ID=USERID FROM USERS WHERE USEREMAIL=@UserEmail AND USERPASSWORD=HASHBYTES('SHA2_512',@Password)
 RETURN @USER_ID
END
GO

INSERT INTO USERS VALUES('U101','Ashley','ashley@gmail.com',9999999999,'10','20',HASHBYTES('SHA2_512','Ashley@123'),1)
INSERT INTO USERS VALUES('U102','Julie','julie@gmail.com',9999999998,'40','20',HASHBYTES('SHA2_512','Julie@123'),0)
INSERT INTO USERS VALUES('U103','Julia','julia@gmail.com',9999999997,'40','20',HASHBYTES('SHA2_512','Julia@123'),0)


CREATE TABLE [dbo].[LaundryRequest] (
    [RequestID]     INT  PRIMARY KEY IDENTITY,
	[UserId]       VARCHAR (20) FOREIGN KEY REFERENCES USERS(UserId),
    [WashingTime]    DATETIME NOT NULL,
    [WhitesOnly]   BIT NOT NULL CHECK([WhitesOnly] IN (0,1)),
    [DenimsOrTrousersOnly]     BIT  NOT NULL CHECK([DenimsOrTrousersOnly] IN (0,1)),
	[GarmentsOnly]    BIT  NOT NULL CHECK([GarmentsOnly] IN (0,1)),
    [UnderGarmentsOnly]    BIT  NOT NULL CHECK([UnderGarmentsOnly] IN (0,1)),
    [Weight] INT  NOT NULL CHECK([Weight] BETWEEN 0 AND 16),
    [WashingMachine]  BIT NOT NULL CHECK([WashingMachine] IN (0,1)),
	[Status] VARCHAR(50) CONSTRAINT chk_LaundryRequest_Status
		CHECK([Status] IN ('Active','Inactive')) 
);
GO


CREATE TABLE MatchedRequest
(
	MatchedRequestId INT PRIMARY KEY IDENTITY,
	OwnerId VARCHAR(20) FOREIGN KEY REFERENCES USERS(UserId),
	WasherId VARCHAR(20) FOREIGN KEY REFERENCES USERS(UserId),
	OwnerRequestId INT FOREIGN KEY REFERENCES LaundryRequest(RequestID),
	WasherRequestId INT FOREIGN KEY REFERENCES LaundryRequest(RequestID),
	[Status] VARCHAR(20) NOT NULL CONSTRAINT chk_MatchedRequest_Status
				CHECK ([Status] IN ('Inactive','Pending','Accepted','Rejected')),
	Distance NUMERIC(25,2) NOT NULL,
	RequestSentBy VARCHAR(20),
	CONSTRAINT uq_RequestsMatched UNIQUE(OwnerId,WasherId,OwnerRequestId,WasherRequestId)
)
GO
CREATE TABLE [dbo].[AcceptedRequest] (
    [AcceptedRequestID]     INT  PRIMARY KEY IDENTITY,
    [OwnerId] VARCHAR(20),
	[WasherId] VARCHAR(20) ,
    [OwnerRequestId] INT ,
	[WasherRequestId] INT ,
	[TimeStamp] DATETIME NOT NULL,
	[Status] VARCHAR(20) NOT NULL CHECK([Status] IN ('Complete','Incomplete','Active')),
	CONSTRAINT fk_RequestsAccepted 
		FOREIGN KEY (OwnerId,WasherId,OwnerRequestId,WasherRequestId) 
		REFERENCES MatchedRequest(OwnerId,WasherId,OwnerRequestId,WasherRequestId)
);
Go

CREATE TABLE [Transaction]
(
	TransactionId INT PRIMARY KEY IDENTITY,
	UserId VARCHAR(20) FOREIGN KEY REFERENCES USERS(UserId),
	Laundrocash INT CONSTRAINT chk_Transaction_Laundrocash CHECK(Laundrocash>0) NOT NULL,
	TransactionType CHAR CONSTRAINT chk_TransactionType 
					CHECK (TransactionType IN ('D','C')) NOT NULL,
	[Message] VARCHAR(50),
	TransactionDateTime DATETIME NOT NULL DEFAULT GETDATE(),
	RequestId INT FOREIGN KEY REFERENCES LaundryRequest(RequestID)
)
GO

CREATE FUNCTION ufn_ViewMatchedRequests(@UserId VARCHAR(20))
RETURNS TABLE
AS
RETURN
(SELECT * FROM [MatchedRequest]  where [OwnerId]=@UserId OR [WasherId]=@UserId)
--Select * from ufn_ViewMatchedRequests('U102')
