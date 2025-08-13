# Configura tu conexión
$server   = "localhost"
$database = "PetCareBookingDb"
$user     = "sa"
$password = 'Nivacathy2033$#'

# Genera dinámicamente GUIDs únicos
$id1 = [guid]::NewGuid()
$id2 = [guid]::NewGuid()
$id3 = [guid]::NewGuid()
$id4 = [guid]::NewGuid()

# Comandos SQL
$sql = @"
INSERT INTO Reservations (Id, PetId, CaregiverId, ClientId, StartDate, EndDate, Note, ReservationStatusId)
VALUES
  ('$id1', NEWID(), NEWID(), NEWID(), GETDATE() + 1, GETDATE() + 2, 'TEST - Pendiente', 1),
  ('$id2', NEWID(), NEWID(), NEWID(), GETDATE() + 3, GETDATE() + 4, 'TEST - Aceptada', 2),
  ('$id3', NEWID(), NEWID(), NEWID(), GETDATE() + 5, GETDATE() + 6, 'TEST - Cancelada', 3),
  ('$id4', NEWID(), NEWID(), NEWID(), GETDATE() - 2, GETDATE() - 1, 'TEST - Finalizada', 4)
"@

# Ejecuta el comando
sqlcmd -S $server -d $database -U $user -P $password -Q $sql -b
