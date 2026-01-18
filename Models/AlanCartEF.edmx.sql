
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/17/2026 14:56:42
-- Generated from EDMX file: D:\Language\Projects\AlanCart\Models\AlanCartEF.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [AlanCart];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[UserData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserData];
GO
IF OBJECT_ID(N'[dbo].[ProductData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductData];
GO
IF OBJECT_ID(N'[dbo].[CartOfUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CartOfUser];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UserData'
CREATE TABLE [dbo].[UserData] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Nickname] nvarchar(max)  NOT NULL,
    [Role] int  NOT NULL
);
GO

-- Creating table 'ProductData'
CREATE TABLE [dbo].[ProductData] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Productname] nvarchar(max)  NOT NULL,
    [Price] int  NOT NULL,
    [Stock] int  NOT NULL,
    [Available] int  NOT NULL,
    [ImgUrl] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CartOfUser'
CREATE TABLE [dbo].[CartOfUser] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [ProductId] int  NOT NULL,
    [Stock] int  NOT NULL,
    [Chekcout] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'UserData'
ALTER TABLE [dbo].[UserData]
ADD CONSTRAINT [PK_UserData]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductData'
ALTER TABLE [dbo].[ProductData]
ADD CONSTRAINT [PK_ProductData]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CartOfUser'
ALTER TABLE [dbo].[CartOfUser]
ADD CONSTRAINT [PK_CartOfUser]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------