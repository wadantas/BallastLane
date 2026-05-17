-- Creates the VehicleStore database and schema
-- Run with SQL Server Management Studio or sqlcmd

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'VehicleStoreDb')
BEGIN
    CREATE DATABASE VehicleStoreDb;
END
GO

USE VehicleStoreDb;
GO
