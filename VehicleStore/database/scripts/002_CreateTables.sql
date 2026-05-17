USE VehicleStoreDb;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id           UNIQUEIDENTIFIER NOT NULL,
        Username     NVARCHAR(100)    NOT NULL,
        Email        NVARCHAR(256)    NOT NULL,
        PasswordHash NVARCHAR(500)    NOT NULL,
        Role         NVARCHAR(50)     NOT NULL,
        CreatedAt    DATETIME2        NOT NULL,
        CONSTRAINT PK_Users PRIMARY KEY (Id),
        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT CK_Users_Role CHECK (Role IN ('User', 'Admin'))
    );
END
GO

IF OBJECT_ID(N'dbo.Vehicles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Vehicles
    (
        Id          UNIQUEIDENTIFIER NOT NULL,
        PlateNumber NVARCHAR(20)     NOT NULL,
        Document    NVARCHAR(50)     NOT NULL,
        Brand       NVARCHAR(100)    NOT NULL,
        Model       NVARCHAR(100)    NOT NULL,
        Year        INT              NOT NULL,
        Price       DECIMAL(18, 2)   NOT NULL,
        IsSold      BIT              NOT NULL CONSTRAINT DF_Vehicles_IsSold DEFAULT (0),
        CreatedAt   DATETIME2        NOT NULL,
        UpdatedAt   DATETIME2        NULL,
        CONSTRAINT PK_Vehicles PRIMARY KEY (Id),
        CONSTRAINT UQ_Vehicles_PlateNumber UNIQUE (PlateNumber),
        CONSTRAINT CK_Vehicles_Year CHECK (Year >= 1900),
        CONSTRAINT CK_Vehicles_Price CHECK (Price > 0)
    );
END
GO

CREATE INDEX IX_Vehicles_IsSold ON dbo.Vehicles (IsSold);
GO
