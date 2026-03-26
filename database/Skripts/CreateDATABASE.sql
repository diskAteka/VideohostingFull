-- Удаление и создание базы данных
USE master;
GO

ALTER DATABASE videohostingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE IF EXISTS videohostingDB;

-- Создание базы данных с параметрами из второго запроса
CREATE DATABASE [videohostingDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'videohostingDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.VIDEOHOSTINGSQL\MSSQL\DATA\videohostingDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'videohostingDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.VIDEOHOSTINGSQL\MSSQL\DATA\videohostingDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF;
GO

USE videohostingDB;
GO

-- Настройка параметров базы данных (из второго запроса)
ALTER DATABASE [videohostingDB] SET COMPATIBILITY_LEVEL = 170;
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
BEGIN
    EXEC [videohostingDB].[dbo].[sp_fulltext_database] @action = 'enable';
END
GO
ALTER DATABASE [videohostingDB] SET ANSI_NULL_DEFAULT OFF;
ALTER DATABASE [videohostingDB] SET ANSI_NULLS OFF;
ALTER DATABASE [videohostingDB] SET ANSI_PADDING OFF;
ALTER DATABASE [videohostingDB] SET ANSI_WARNINGS OFF;
ALTER DATABASE [videohostingDB] SET ARITHABORT OFF;
ALTER DATABASE [videohostingDB] SET AUTO_CLOSE OFF;
ALTER DATABASE [videohostingDB] SET AUTO_SHRINK OFF;
ALTER DATABASE [videohostingDB] SET AUTO_UPDATE_STATISTICS ON;
ALTER DATABASE [videohostingDB] SET CURSOR_CLOSE_ON_COMMIT OFF;
ALTER DATABASE [videohostingDB] SET CURSOR_DEFAULT GLOBAL;
ALTER DATABASE [videohostingDB] SET CONCAT_NULL_YIELDS_NULL OFF;
ALTER DATABASE [videohostingDB] SET NUMERIC_ROUNDABORT OFF;
ALTER DATABASE [videohostingDB] SET QUOTED_IDENTIFIER OFF;
ALTER DATABASE [videohostingDB] SET RECURSIVE_TRIGGERS OFF;
ALTER DATABASE [videohostingDB] SET ENABLE_BROKER;
ALTER DATABASE [videohostingDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF;
ALTER DATABASE [videohostingDB] SET DATE_CORRELATION_OPTIMIZATION OFF;
ALTER DATABASE [videohostingDB] SET TRUSTWORTHY OFF;
ALTER DATABASE [videohostingDB] SET ALLOW_SNAPSHOT_ISOLATION OFF;
ALTER DATABASE [videohostingDB] SET PARAMETERIZATION SIMPLE;
ALTER DATABASE [videohostingDB] SET READ_COMMITTED_SNAPSHOT OFF;
ALTER DATABASE [videohostingDB] SET HONOR_BROKER_PRIORITY OFF;
ALTER DATABASE [videohostingDB] SET RECOVERY FULL;
ALTER DATABASE [videohostingDB] SET MULTI_USER;
ALTER DATABASE [videohostingDB] SET PAGE_VERIFY CHECKSUM;
ALTER DATABASE [videohostingDB] SET DB_CHAINING OFF;
ALTER DATABASE [videohostingDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF );
ALTER DATABASE [videohostingDB] SET TARGET_RECOVERY_TIME = 60 SECONDS;
ALTER DATABASE [videohostingDB] SET DELAYED_DURABILITY = DISABLED;
ALTER DATABASE [videohostingDB] SET ACCELERATED_DATABASE_RECOVERY = OFF;
ALTER DATABASE [videohostingDB] SET OPTIMIZED_LOCKING = OFF;
ALTER DATABASE [videohostingDB] SET QUERY_STORE = ON;
ALTER DATABASE [videohostingDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON);
GO

-- Таблица User (из первого запроса с модификаторами)
CREATE TABLE [dbo].[User]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [PasswordHash] CHAR(64) NOT NULL, -- Для SHA256 хэша
    [Email] NVARCHAR(254) NOT NULL,
    [CanUpload] BIT NOT NULL DEFAULT 0,
    [PasswordSalt] CHAR(32) NOT NULL, -- Соль для пароля
    [RegisteredAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1
);

-- Индексы для User
CREATE UNIQUE INDEX IX_User_Email ON [dbo].[User]([Email]);
CREATE INDEX IX_User_Name ON [dbo].[User]([Name]);

-- Таблица Employee (из второго запроса)
CREATE TABLE [dbo].[Employee]
(
    [Id] INT NOT NULL,
    [UserName] NVARCHAR(50) NOT NULL,
    [Password] NVARCHAR(50) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL CHECK ([Role] IN ('Admin', 'Verifier')),
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Таблица Video (из первого запроса с модификаторами)
CREATE TABLE [dbo].[Video]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(2000),
    [DateUpload] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Link] NVARCHAR(500) NOT NULL,
    [Poster] NVARCHAR(500) NOT NULL,
    [Likes] INT NOT NULL DEFAULT 0,
    [Dislikes] INT NOT NULL DEFAULT 0,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [Views] INT NOT NULL DEFAULT 0,
    [AuthorId] INT NOT NULL,
    FOREIGN KEY (AuthorId) 
        REFERENCES [dbo].[User](Id) 
        ON DELETE NO ACTION
);

-- Индексы для Video
CREATE INDEX IX_Video_Name ON [dbo].[Video]([Name]);
CREATE INDEX IX_Video_DateUpload ON [dbo].[Video]([DateUpload]);
CREATE INDEX IX_Video_IsVerified ON [dbo].[Video]([IsVerified]);

-- Таблица Comment (из первого запроса с модификаторами)
CREATE TABLE [dbo].[Comment]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [VideoId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Text] NVARCHAR(1000) NOT NULL,
    [Date] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (VideoId) 
        REFERENCES [dbo].[Video](Id) 
        ON DELETE CASCADE,
    FOREIGN KEY (UserId) 
        REFERENCES [dbo].[User](Id) 
        ON DELETE NO ACTION
);

-- Индексы для Comment
CREATE INDEX IX_Comment_Date ON [dbo].[Comment]([Date]);
CREATE INDEX IX_Comment_VideoId_Date ON [dbo].[Comment]([VideoId], [Date]);

-- Таблица ServerLog (из первого запроса с модификаторами)
CREATE TABLE [dbo].[ServerLog]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [Type] NVARCHAR NOT NULL,
    [Date] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) 
        REFERENCES [dbo].[User](Id) 
        ON DELETE CASCADE
);

-- Индексы для ServerLog
CREATE INDEX IX_ServerLog_Date ON [dbo].[ServerLog]([Date]);
CREATE INDEX IX_ServerLog_UserId_Date ON [dbo].[ServerLog]([UserId], [Date]);
CREATE INDEX IX_ServerLog_Type ON [dbo].[ServerLog]([Type]);

-- Таблица Like (из первого запроса с модификаторами)
CREATE TABLE [dbo].[Like]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [VideoId] INT NOT NULL,
    [UserId] INT NOT NULL,
    FOREIGN KEY (VideoId) 
        REFERENCES [dbo].[Video](Id) 
        ON DELETE CASCADE,
    FOREIGN KEY (UserId) 
        REFERENCES [dbo].[User](Id) 
        ON DELETE NO ACTION
);

-- Уникальный индекс для Like
CREATE UNIQUE INDEX IX_Like_VideoId_UserId ON [dbo].[Like]([VideoId], [UserId]);

-- Таблица DisLike (из первого запроса с модификаторами)
CREATE TABLE [dbo].[DisLike]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [VideoId] INT NOT NULL,
    [UserId] INT NOT NULL,
    FOREIGN KEY (VideoId) 
        REFERENCES [dbo].[Video](Id) 
        ON DELETE CASCADE,
    FOREIGN KEY (UserId) 
        REFERENCES [dbo].[User](Id) 
        ON DELETE NO ACTION
);

-- Уникальный индекс для DisLike
CREATE UNIQUE INDEX IX_DisLike_VideoId_UserId ON [dbo].[DisLike]([VideoId], [UserId]);

-- Таблица View (из первого запроса с модификаторами)
CREATE TABLE [dbo].[View]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [VideoId] INT NOT NULL,
    [UserId] INT NOT NULL,
    FOREIGN KEY (VideoId) 
        REFERENCES [dbo].[Video](Id) 
        ON DELETE CASCADE,
    FOREIGN KEY (UserId) 
        REFERENCES [dbo].[User](Id) 
        ON DELETE NO ACTION
);

-- Уникальный индекс для View
CREATE UNIQUE INDEX IX_View_VideoId_UserId ON [dbo].[View]([VideoId], [UserId]);

-- Очистка существующих пользователей и логинов
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'admin_user')
    DROP USER admin_user;
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'main_server_user')
    DROP USER main_server_user;
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'download_server_user')
    DROP USER download_server_user;
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'viewing_server_user')
    DROP USER viewing_server_user;

USE master;
IF EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'admin_login')
    DROP LOGIN admin_login;
IF EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'main_server_login')
    DROP LOGIN main_server_login;
IF EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'download_server_login')
    DROP LOGIN download_server_login;
IF EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'viewing_server_login')
    DROP LOGIN viewing_server_login;
GO

USE videohostingDB;
GO

-- Создание логинов и пользователей
CREATE LOGIN admin_login WITH PASSWORD = 'admin';
CREATE USER admin_user FOR LOGIN admin_login;
ALTER ROLE db_owner ADD MEMBER admin_user;

CREATE LOGIN main_server_login WITH PASSWORD = 'main_server';
CREATE USER main_server_user FOR LOGIN main_server_login;

CREATE LOGIN download_server_login WITH PASSWORD = 'download_server';
CREATE USER download_server_user FOR LOGIN download_server_login;

CREATE LOGIN viewing_server_login WITH PASSWORD = 'viewing_server';
CREATE USER viewing_server_user FOR LOGIN viewing_server_login;

-- Назначение прав для main_server_user (чтение/запись всех таблиц)
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[User] TO main_server_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Employee] TO main_server_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[ServerLog] TO main_server_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Video] TO main_server_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Comment] TO main_server_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Like] TO main_server_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[DisLike] TO main_server_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[View] TO main_server_user;

-- Права для download_server_user (запись в video, просмотр user)
GRANT SELECT ON [dbo].[User] TO download_server_user;
GRANT INSERT, UPDATE ON [dbo].[Video] TO download_server_user;

-- Права для viewing_server_user (чтение всех таблиц, вставка в View)
GRANT SELECT ON [dbo].[Video] TO viewing_server_user;
GRANT SELECT ON [dbo].[Comment] TO viewing_server_user;
GRANT SELECT ON [dbo].[Like] TO viewing_server_user;
GRANT SELECT ON [dbo].[DisLike] TO viewing_server_user;
GRANT SELECT ON [dbo].[View] TO viewing_server_user;
GRANT SELECT ON [dbo].[User] TO viewing_server_user;
GRANT SELECT ON [dbo].[Employee] TO viewing_server_user;
GRANT INSERT ON [dbo].[View] TO viewing_server_user;

-- Включение логинов
ALTER LOGIN admin_login ENABLE;
ALTER LOGIN main_server_login ENABLE;
ALTER LOGIN download_server_login ENABLE;
ALTER LOGIN viewing_server_login ENABLE;

-- Настройка удаленных соединений
EXEC sp_configure 'remote admin connections', 1;
RECONFIGURE;

-- Установка базы в режим READ_WRITE
ALTER DATABASE [videohostingDB] SET READ_WRITE;
GO

PRINT 'База данных videohostingDB успешно создана со всеми таблицами и пользователями';