-- Crear usuario si no existe
IF NOT EXISTS (SELECT name FROM sys.sql_logins WHERE name = 'petcare_user')
BEGIN
    CREATE LOGIN petcare_user WITH PASSWORD = 'Nivacathy2033$#';
    CREATE USER petcare_user FOR LOGIN petcare_user;
END

-- Crear base de datos si no existe
IF DB_ID('PetCareAuthDb') IS NULL
    CREATE DATABASE PetCareAuthDb;

IF DB_ID('PetCarePetsDb') IS NULL
    CREATE DATABASE PetCarePetsDb;

IF DB_ID('PetCareBookingDb') IS NULL
    CREATE DATABASE PetCareBookingDb;

IF DB_ID('PetCareReviewDb') IS NULL
    CREATE DATABASE PetCareReviewDb;

IF DB_ID('PetCareNotificationDb') IS NULL
    CREATE DATABASE PetCareNotificationDb;

IF DB_ID('PetCarePaymentDb') IS NULL
    CREATE DATABASE PetCarePaymentDb;

-- Asignar rol al usuario en cada DB
USE PetCareAuthDb;
EXEC sp_addrolemember N'db_owner', N'petcare_user';

USE PetCarePetsDb;
EXEC sp_addrolemember N'db_owner', N'petcare_user';

USE PetCareBookingDb;
EXEC sp_addrolemember N'db_owner', N'petcare_user';

USE PetCareReviewDb;
EXEC sp_addrolemember N'db_owner', N'petcare_user';

USE PetCareNotificationDb;
EXEC sp_addrolemember N'db_owner', N'petcare_user';

USE PetCarePaymentDb;
EXEC sp_addrolemember N'db_owner', N'petcare_user';
