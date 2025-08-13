-- Crear usuario
CREATE LOGIN petcare_user WITH PASSWORD = 'Nivacathy2033$#';

-- Bases de datos
DECLARE @Databases TABLE (DbName NVARCHAR(100));
INSERT INTO @Databases (DbName) VALUES
  ('PetCareAuthDb'),
  ('PetCarePetsDb'),
  ('PetCareBookingDb'),
  ('PetCareReviewDb'),
  ('PetCareNotificationDb'),
  ('PetCarePaymentDb');

DECLARE @DbName NVARCHAR(100);
DECLARE @Cmd NVARCHAR(MAX);

-- Crear cada base y asignar el usuario
DECLARE db_cursor CURSOR FOR SELECT DbName FROM @Databases;
OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @DbName;

WHILE @@FETCH_STATUS = 0
BEGIN
  SET @Cmd = '
    IF DB_ID(''' + @DbName + ''') IS NULL
    BEGIN
      CREATE DATABASE [' + @DbName + '];
    END;
    USE [' + @DbName + '];
    CREATE USER petcare_user FOR LOGIN petcare_user;
    EXEC sp_addrolemember ''db_owner'', ''petcare_user'';
  ';
  EXEC(@Cmd);
  FETCH NEXT FROM db_cursor INTO @DbName;
END;

CLOSE db_cursor;
DEALLOCATE db_cursor;
