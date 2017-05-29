
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/18/2017 17:58:42
-- Generated from EDMX file: C:\Users\Alexandra\Desktop\OnlineShopping\OnlineShopping\DAL\Online_Shopping.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Online_Shopping];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__Tbl_Cart__CartSt__30F848ED]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_Cart] DROP CONSTRAINT [FK__Tbl_Cart__CartSt__30F848ED];
GO
IF OBJECT_ID(N'[dbo].[FK__Tbl_Cart__Produc__31EC6D26]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_Cart] DROP CONSTRAINT [FK__Tbl_Cart__Produc__31EC6D26];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Tbl_Cart]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Cart];
GO
IF OBJECT_ID(N'[dbo].[Tbl_CartStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_CartStatus];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Category];
GO
IF OBJECT_ID(N'[dbo].[Tbl_MemberRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_MemberRole];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Members]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Members];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Product]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Product];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Roles];
GO
IF OBJECT_ID(N'[dbo].[Tbl_ShippingDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_ShippingDetails];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Tbl_CartStatus'
CREATE TABLE [dbo].[Tbl_CartStatus] (
    [CartStatusId] int IDENTITY(1,1) NOT NULL,
    [CartStatus] varchar(100)  NULL
);
GO

-- Creating table 'Tbl_Category'
CREATE TABLE [dbo].[Tbl_Category] (
    [CategoryId] int IDENTITY(1,1) NOT NULL,
    [CategoryName] nvarchar(100)  NULL,
    [IsActive] bit  NULL,
    [IsDelete] bit  NULL
);
GO

-- Creating table 'Tbl_MemberRole'
CREATE TABLE [dbo].[Tbl_MemberRole] (
    [MemberRoleId] int IDENTITY(1,1) NOT NULL,
    [MemberId] int  NULL,
    [RoleId] int  NULL
);
GO

-- Creating table 'Tbl_Members'
CREATE TABLE [dbo].[Tbl_Members] (
    [MemberId] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(50)  NULL,
    [LastName] nvarchar(50)  NULL,
    [EmailId] nvarchar(200)  NULL,
    [Password] nvarchar(50)  NULL,
    [IsActive] bit  NULL,
    [IsDelete] bit  NULL,
    [CreatedOn] datetime  NULL,
    [ModifiedOn] datetime  NULL
);
GO

-- Creating table 'Tbl_Product'
CREATE TABLE [dbo].[Tbl_Product] (
    [ProductId] int IDENTITY(1,1) NOT NULL,
    [ProductName] varchar(100)  NULL,
    [CategoryId] int  NULL,
    [IsActive] bit  NULL,
    [IsDelete] bit  NULL,
    [CreatedDate] datetime  NULL,
    [ModifiedDate] datetime  NULL,
    [Description] varchar(500)  NULL,
    [ProductImage] varchar(50)  NULL,
    [Price] decimal(18,2)  NULL,
    [IsFeatured] bit  NULL
);
GO

-- Creating table 'Tbl_Roles'
CREATE TABLE [dbo].[Tbl_Roles] (
    [RoleId] int IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(50)  NULL
);
GO

-- Creating table 'Tbl_ShippingDetails'
CREATE TABLE [dbo].[Tbl_ShippingDetails] (
    [ShippingDetailId] int IDENTITY(1,1) NOT NULL,
    [MemberId] int  NULL,
    [AddressLine] varchar(100)  NULL,
    [City] varchar(50)  NULL,
    [State] varchar(50)  NULL,
    [Country] varchar(50)  NULL,
    [ZipCode] varchar(50)  NULL,
    [OrderId] varchar(50)  NULL,
    [AmountPaid] decimal(18,0)  NULL,
    [PaymentType] varchar(50)  NULL
);
GO

-- Creating table 'Tbl_Cart'
CREATE TABLE [dbo].[Tbl_Cart] (
    [CartId] int IDENTITY(1,1) NOT NULL,
    [ProductId] int  NULL,
    [MemberId] int  NULL,
    [CartStatusId] int  NULL,
    [AddedOn] datetime  NULL,
    [UpdatedOn] datetime  NULL,
    [ShippingDetailId] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [CartStatusId] in table 'Tbl_CartStatus'
ALTER TABLE [dbo].[Tbl_CartStatus]
ADD CONSTRAINT [PK_Tbl_CartStatus]
    PRIMARY KEY CLUSTERED ([CartStatusId] ASC);
GO

-- Creating primary key on [CategoryId] in table 'Tbl_Category'
ALTER TABLE [dbo].[Tbl_Category]
ADD CONSTRAINT [PK_Tbl_Category]
    PRIMARY KEY CLUSTERED ([CategoryId] ASC);
GO

-- Creating primary key on [MemberRoleId] in table 'Tbl_MemberRole'
ALTER TABLE [dbo].[Tbl_MemberRole]
ADD CONSTRAINT [PK_Tbl_MemberRole]
    PRIMARY KEY CLUSTERED ([MemberRoleId] ASC);
GO

-- Creating primary key on [MemberId] in table 'Tbl_Members'
ALTER TABLE [dbo].[Tbl_Members]
ADD CONSTRAINT [PK_Tbl_Members]
    PRIMARY KEY CLUSTERED ([MemberId] ASC);
GO

-- Creating primary key on [ProductId] in table 'Tbl_Product'
ALTER TABLE [dbo].[Tbl_Product]
ADD CONSTRAINT [PK_Tbl_Product]
    PRIMARY KEY CLUSTERED ([ProductId] ASC);
GO

-- Creating primary key on [RoleId] in table 'Tbl_Roles'
ALTER TABLE [dbo].[Tbl_Roles]
ADD CONSTRAINT [PK_Tbl_Roles]
    PRIMARY KEY CLUSTERED ([RoleId] ASC);
GO

-- Creating primary key on [ShippingDetailId] in table 'Tbl_ShippingDetails'
ALTER TABLE [dbo].[Tbl_ShippingDetails]
ADD CONSTRAINT [PK_Tbl_ShippingDetails]
    PRIMARY KEY CLUSTERED ([ShippingDetailId] ASC);
GO

-- Creating primary key on [CartId] in table 'Tbl_Cart'
ALTER TABLE [dbo].[Tbl_Cart]
ADD CONSTRAINT [PK_Tbl_Cart]
    PRIMARY KEY CLUSTERED ([CartId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [CategoryId] in table 'Tbl_Product'
ALTER TABLE [dbo].[Tbl_Product]
ADD CONSTRAINT [FK_Tbl_Category_Tbl_Product]
    FOREIGN KEY ([CategoryId])
    REFERENCES [dbo].[Tbl_Category]
        ([CategoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tbl_Category_Tbl_Product'
CREATE INDEX [IX_FK_Tbl_Category_Tbl_Product]
ON [dbo].[Tbl_Product]
    ([CategoryId]);
GO

-- Creating foreign key on [MemberId] in table 'Tbl_MemberRole'
ALTER TABLE [dbo].[Tbl_MemberRole]
ADD CONSTRAINT [fk_Tbl_Member_Tbl_MemberRole]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Tbl_Members]
        ([MemberId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_Tbl_Member_Tbl_MemberRole'
CREATE INDEX [IX_fk_Tbl_Member_Tbl_MemberRole]
ON [dbo].[Tbl_MemberRole]
    ([MemberId]);
GO

-- Creating foreign key on [RoleId] in table 'Tbl_MemberRole'
ALTER TABLE [dbo].[Tbl_MemberRole]
ADD CONSTRAINT [fk_Tbl_Roles_Tbl_MemberRole]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Tbl_Roles]
        ([RoleId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_Tbl_Roles_Tbl_MemberRole'
CREATE INDEX [IX_fk_Tbl_Roles_Tbl_MemberRole]
ON [dbo].[Tbl_MemberRole]
    ([RoleId]);
GO

-- Creating foreign key on [CartStatusId] in table 'Tbl_Cart'
ALTER TABLE [dbo].[Tbl_Cart]
ADD CONSTRAINT [FK__Tbl_Cart__CartSt__3F466844]
    FOREIGN KEY ([CartStatusId])
    REFERENCES [dbo].[Tbl_CartStatus]
        ([CartStatusId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Tbl_Cart__CartSt__3F466844'
CREATE INDEX [IX_FK__Tbl_Cart__CartSt__3F466844]
ON [dbo].[Tbl_Cart]
    ([CartStatusId]);
GO

-- Creating foreign key on [MemberId] in table 'Tbl_Cart'
ALTER TABLE [dbo].[Tbl_Cart]
ADD CONSTRAINT [FK__Tbl_Cart__Member__3C69FB99]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Tbl_Members]
        ([MemberId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Tbl_Cart__Member__3C69FB99'
CREATE INDEX [IX_FK__Tbl_Cart__Member__3C69FB99]
ON [dbo].[Tbl_Cart]
    ([MemberId]);
GO

-- Creating foreign key on [ProductId] in table 'Tbl_Cart'
ALTER TABLE [dbo].[Tbl_Cart]
ADD CONSTRAINT [FK__Tbl_Cart__Produc__3E52440B]
    FOREIGN KEY ([ProductId])
    REFERENCES [dbo].[Tbl_Product]
        ([ProductId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Tbl_Cart__Produc__3E52440B'
CREATE INDEX [IX_FK__Tbl_Cart__Produc__3E52440B]
ON [dbo].[Tbl_Cart]
    ([ProductId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------