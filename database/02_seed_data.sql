
USE SportsCenterDB;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- =============================================
-- 1. INSERT 2 CENTERS
-- =============================================
IF NOT EXISTS (
    SELECT 1 FROM dbo.Center WHERE name = N'FPT Sports Center'
)
BEGIN
    INSERT INTO dbo.Center (name, address, hotline, email)
    VALUES (
        N'FPT Sports Center',
        N'FPT University, Hoa Lac, Hanoi',
        '02473001866',
        'fpt.center@example.com'
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Center WHERE name = N'Sunrise Sports Center'
)
BEGIN
    INSERT INTO dbo.Center (name, address, hotline, email)
    VALUES (
        N'Sunrise Sports Center',
        N'District 7, Ho Chi Minh City',
        '02873001234',
        'sunrise.center@example.com'
    );
END;

-- =============================================
-- 2. INSERT 4 ROLES
-- =============================================
IF NOT EXISTS (
    SELECT 1 FROM dbo.Role WHERE name = 'CENTER_MANAGER'
)
    INSERT INTO dbo.Role (name, description)
    VALUES ('CENTER_MANAGER', N'Quản lý trung tâm');

IF NOT EXISTS (
    SELECT 1 FROM dbo.Role WHERE name = 'COACH'
)
    INSERT INTO dbo.Role (name, description)
    VALUES ('COACH', N'Huấn luyện viên');

IF NOT EXISTS (
    SELECT 1 FROM dbo.Role WHERE name = 'MEMBER'
)
    INSERT INTO dbo.Role (name, description)
    VALUES ('MEMBER', N'Thành viên');

IF NOT EXISTS (
    SELECT 1 FROM dbo.Role WHERE name = 'RECEPTIONIST'
)
    INSERT INTO dbo.Role (name, description)
    VALUES ('RECEPTIONIST', N'Nhân viên lễ tân');

-- =============================================
-- 3. INSERT 5 USERS
-- Demo password for all accounts: Demo123!
-- Hashes are ASP.NET Core Identity V3 format.
-- =============================================

-- CENTER_MANAGER
IF NOT EXISTS (
    SELECT 1 FROM dbo.[User] WHERE username = 'manager01'
)
BEGIN
    INSERT INTO dbo.[User] (
        role_id, center_id, username, password_hash,
        full_name, qualification, email, phone,
        date_of_birth, gender, status
    )
    SELECT
        r.id, c.id, 'manager01',
        'AQAAAAIAAYagAAAAEAIkp8Vo/Sr2nMZ9wRd2pUG92NaiyFRqf/mS3VR+U2aRz2tj73hgptFOu2tLf31jZQ==',
        N'Nguyen Van Hung', NULL,
        'manager01@example.com', '0901000001',
        '2000-01-15', N'Male', 'ACTIVE'
    FROM dbo.Role r
    CROSS JOIN dbo.Center c
    WHERE r.name = 'CENTER_MANAGER'
      AND c.name = N'FPT Sports Center';
END;

-- COACH
IF NOT EXISTS (
    SELECT 1 FROM dbo.[User] WHERE username = 'coach01'
)
BEGIN
    INSERT INTO dbo.[User] (
        role_id, center_id, username, password_hash,
        full_name, qualification, email, phone,
        date_of_birth, gender, status
    )
    SELECT
        r.id, c.id, 'coach01',
        'AQAAAAIAAYagAAAAEMxU9f+4znkv681hDvga1+9ezVX2s4VtDxFkqRRdZoynBKlnNeI66miAEmo8p9+1TQ==',
        N'Tran Minh Thinh', N'Personal Trainer Certificate',
        'coach01@example.com', '0901000002',
        '1998-05-20', N'Male', 'ACTIVE'
    FROM dbo.Role r
    CROSS JOIN dbo.Center c
    WHERE r.name = 'COACH'
      AND c.name = N'FPT Sports Center';
END;

-- RECEPTIONIST
IF NOT EXISTS (
    SELECT 1 FROM dbo.[User] WHERE username = 'reception01'
)
BEGIN
    INSERT INTO dbo.[User] (
        role_id, center_id, username, password_hash,
        full_name, qualification, email, phone,
        date_of_birth, gender, status
    )
    SELECT
        r.id, c.id, 'reception01',
        'AQAAAAIAAYagAAAAEFB2a8H12YkCXZQIzmxBsFSgz3/2x7T/15PBu2b+t9dmGByL00stE8CyP7c9Ydp8Sw==',
        N'Le Thu Nguyen', NULL,
        'reception01@example.com', '0901000003',
        '2001-09-12', N'Female', 'ACTIVE'
    FROM dbo.Role r
    CROSS JOIN dbo.Center c
    WHERE r.name = 'RECEPTIONIST'
      AND c.name = N'Sunrise Sports Center';
END;

-- MEMBER 1
IF NOT EXISTS (
    SELECT 1 FROM dbo.[User] WHERE username = 'member01'
)
BEGIN
    INSERT INTO dbo.[User] (
        role_id, center_id, username, password_hash,
        full_name, qualification, email, phone,
        date_of_birth, gender, status
    )
    SELECT
        r.id, c.id, 'member01',
        'AQAAAAIAAYagAAAAEHKnDeZRQFcmFFEMl7QWTa7mniCAdLOBQg5T2VmcZv+FMb0yE+2k2+VtkkX0aF9wcw==',
        N'Pham Minh Bao', NULL,
        'member01@example.com', '0901000004',
        '2003-03-10', N'Male', 'ACTIVE'
    FROM dbo.Role r
    CROSS JOIN dbo.Center c
    WHERE r.name = 'MEMBER'
      AND c.name = N'FPT Sports Center';
END;

-- MEMBER 2
IF NOT EXISTS (
    SELECT 1 FROM dbo.[User] WHERE username = 'member02'
)
BEGIN
    INSERT INTO dbo.[User] (
        role_id, center_id, username, password_hash,
        full_name, qualification, email, phone,
        date_of_birth, gender, status
    )
    SELECT
        r.id, c.id, 'member02',
        'AQAAAAIAAYagAAAAEMxg3TVkMFeZVrglhNTXZdI38Z/pxAsq4zkaP/ogOg3kb6DibhYG7WCKB+GbM48GKg==',
        N'Vo Thanh An', NULL,
        'member02@example.com', '0901000005',
        '2002-11-25', N'Male', 'ACTIVE'
    FROM dbo.Role r
    CROSS JOIN dbo.Center c
    WHERE r.name = 'MEMBER'
      AND c.name = N'Sunrise Sports Center';
END;

COMMIT TRANSACTION;
GO
