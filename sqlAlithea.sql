/****** Script for SelectTopNRows command from SSMS  ******/
ALTER PROCEDURE UserLogin (@UserName nvarchar(250), @Password nvarchar(250))
AS
BEGIN
	DECLARE @count int
	DECLARE @res bit
	SELECT @count = COUNT(*) FROM UserAccounts u WHERE u.Username = @UserName AND u.Password = @Password;
	if(@count > 0)
		SET @res = 1
	else
		SET @res = 0

	SELECT @res
END

///////////////////////////
ALTER TRIGGER InsertProduct ON Products
FOR INSERT
AS
BEGIN
	DECLARE @count INT, @createAt DATETIME, @updateAt DATETIME, @price float
	SELECT @count = p.Quantity, @price = p.UnitPrice, @createAt = p.CreatedAt, @updateAt = p.UpdatedAt FROM inserted p
	IF(@count < 0 OR @price < 0 OR @createAt > @updateAt)
	BEGIN
		ROLLBACK TRANSACTION
	END
END


///////////////////////////////////
CREATE TRIGGER UpdateProduct ON Products
AFTER UPDATE 
AS
BEGIN
	DECLARE @count INT,  @price float
	SELECT @count = COUNT(*) FROM Products p WHERE p.Quantity < 0 
	SELECT @price = COUNT(*) FROM Products p WHERE p.UnitPrice < 0 
	IF(@count > 0 OR @price > 0)
	BEGIN
		ROLLBACK TRANSACTION
	END
END

/////////////////////
CREATE TRIGGER UpdateAttribute ON Attributes
AFTER UPDATE
AS
BEGIN
	DECLARE @count INT,  @price float
	SELECT @count = COUNT(*) FROM Attributes a WHERE a.Quantity < 0 
	SELECT @price = COUNT(*) FROM Attributes a WHERE a.UnitPrice < 0 
	IF(@count > 0 OR @price > 0)
	BEGIN
		ROLLBACK TRANSACTION
	END
END
