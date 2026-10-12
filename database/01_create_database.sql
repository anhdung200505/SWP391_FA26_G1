
-- =============================================
-- 1. DROP AND CREATE DATABASE
-- WARNING: This deletes the existing database!
-- =============================================
USE master;
GO

CREATE DATABASE SportsCenterDB;
GO

USE SportsCenterDB;
GO

-- =============================================
-- 2. TABLE: Center
-- =============================================
CREATE TABLE dbo.Center (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL,
    address NVARCHAR(255) NOT NULL,
    hotline VARCHAR(20) NULL,
    email VARCHAR(254) NULL
);
GO

-- =============================================
-- 3. TABLE: Role
-- =============================================
CREATE TABLE dbo.Role (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description NVARCHAR(255) NULL,
    create_at DATETIME2 NOT NULL
        DEFAULT SYSDATETIME(),
    updated_at DATETIME2 NOT NULL
        DEFAULT SYSDATETIME()
);
GO

-- =============================================
-- 4. TABLE: User
-- =============================================
CREATE TABLE dbo.[User] (
    id INT IDENTITY(1,1) PRIMARY KEY,

    role_id INT NOT NULL,
    center_id INT NOT NULL,

    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name NVARCHAR(150) NOT NULL,
    qualification NVARCHAR(255) NULL,

    email VARCHAR(254) NOT NULL UNIQUE,
    phone VARCHAR(20) NULL,
    date_of_birth DATE NULL,
    gender NVARCHAR(20) NULL,

    status VARCHAR(20) NOT NULL
        DEFAULT 'ACTIVE',

    created_at DATETIME2 NOT NULL
        DEFAULT SYSDATETIME(),

    CONSTRAINT FK_User_Role
        FOREIGN KEY (role_id)
        REFERENCES dbo.Role(id),

    CONSTRAINT FK_User_Center
        FOREIGN KEY (center_id)
        REFERENCES dbo.Center(id),

    CONSTRAINT CK_User_Status
        CHECK (status IN ('ACTIVE', 'INACTIVE', 'SUSPENDED'))
);
GO

-- =============================================
-- 5. VERIFY TABLES
-- =============================================
SELECT
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('Center', 'Role', 'User')
ORDER BY TABLE_NAME, ORDINAL_POSITION;
GO
