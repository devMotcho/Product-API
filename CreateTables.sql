CREATE DATABASE MyProductsDatabase
GO

USE MyProductsDatabase
GO

CREATE SCHEMA MyAppSchema

CREATE TABLE MyAppSchema.Products
(
    ProductId INT IDENTITY(1, 1) PRIMARY KEY
    ,Name NVARCHAR(50)
    ,Description NVARCHAR(50)
    ,Price DECIMAL(18, 4)
    ,Active BIT
);

CREATE TABLE MyAppSchema.Users
(
    UserId INT IDENTITY(1, 1) PRIMARY KEY
    ,FirstName NVARCHAR(50)
    ,LastName NVARCHAR(50)
    ,Email NVARCHAR(50)
    ,Gender NVARCHAR(50)
    ,Active BIT
);

CREATE TABLE MyAppSchema.Auth
(
    Email NVARCHAR(50)
    , PasswordHash VARBINARY(MAX)
    , PasswordSalt VARBINARY(MAX)
);
