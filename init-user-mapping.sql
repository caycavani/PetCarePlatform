-- Asociar petcare_user a PetCareAuthDb
USE PetCareAuthDb;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'petcare_user')
BEGIN
    CREATE USER [petcare_user] FOR LOGIN [petcare_user];
    ALTER ROLE [db_owner] ADD MEMBER [petcare_user];
    PRINT '✅ Usuario petcare_user creado en PetCareAuthDb';
END
ELSE
    PRINT 'ℹ️ Usuario petcare_user ya existe en PetCareAuthDb';
GO

-- Asociar petcare_user a PetCarePetsDb
USE PetCarePetsDb;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'petcare_user')
BEGIN
    CREATE USER [petcare_user] FOR LOGIN [petcare_user];
    ALTER ROLE [db_owner] ADD MEMBER [petcare_user];
    PRINT '✅ Usuario petcare_user creado en PetCarePetsDb';
END
ELSE
    PRINT 'ℹ️ Usuario petcare_user ya existe en PetCarePetsDb';
GO

-- Asociar petcare_user a PetCareBookingDb
USE PetCareBookingDb;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'petcare_user')
BEGIN
    CREATE USER [petcare_user] FOR LOGIN [petcare_user];
    ALTER ROLE [db_owner] ADD MEMBER [petcare_user];
    PRINT '✅ Usuario petcare_user creado en PetCareBookingDb';
END
ELSE
    PRINT 'ℹ️ Usuario petcare_user ya existe en PetCareBookingDb';
GO

-- Asociar petcare_user a PetCareReviewDb
USE PetCareReviewDb;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'petcare_user')
BEGIN
    CREATE USER [petcare_user] FOR LOGIN [petcare_user];
    ALTER ROLE [db_owner] ADD MEMBER [petcare_user];
    PRINT '✅ Usuario petcare_user creado en PetCareReviewDb';
END
ELSE
    PRINT 'ℹ️ Usuario petcare_user ya existe en PetCareReviewDb';
GO

-- Asociar petcare_user a PetCareNotificationDb
USE PetCareNotificationDb;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'petcare_user')
BEGIN
    CREATE USER [petcare_user] FOR LOGIN [petcare_user];
    ALTER ROLE [db_owner] ADD MEMBER [petcare_user];
    PRINT '✅ Usuario petcare_user creado en PetCareNotificationDb';
END
ELSE
    PRINT 'ℹ️ Usuario petcare_user ya existe en PetCareNotificationDb';
GO

-- Asociar petcare_user a PetCarePaymentDb
USE PetCarePaymentDb;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'petcare_user')
BEGIN
    CREATE USER [petcare_user] FOR LOGIN [petcare_user];
    ALTER ROLE [db_owner] ADD MEMBER [petcare_user];
    PRINT '✅ Usuario petcare_user creado en PetCarePaymentDb';
END
ELSE
    PRINT 'ℹ️ Usuario petcare_user ya existe en PetCarePaymentDb';
GO
