
USE SportsCenterDB;
GO

UPDATE dbo.[User]
SET password_hash =
    'AQAAAAIAAYagAAAAEEdYn17UbkdqYdclTXnDfKgmJAk0yMaASpyvmE8BSP9jMgjh4syXh5IHrmcVPlIa9w=='
WHERE username = 'manager01';

UPDATE dbo.[User]
SET password_hash =
    'AQAAAAIAAYagAAAAEKu1df4egV8WM+o4dNfrdN/5onJwASojdWDi38k9NA8yBNYjAKlzOKBsHIpcJAt8IA=='
WHERE username = 'coach01';

UPDATE dbo.[User]
SET password_hash =
    'AQAAAAIAAYagAAAAEAYQmzfs5fDcCwCKhnNVTp6klC4A+IZNSAf2MdGAoe89OSc+n4QMfCYm7DjyCLXO5g=='
WHERE username = 'reception01';

UPDATE dbo.[User]
SET password_hash =
    'AQAAAAIAAYagAAAAENb7+XXmozlgxjTA2UcqM827gMPvzirokTUYhai8sjuWYZ0ORp45ClrM3grROuDbmQ=='
WHERE username = 'member01';

UPDATE dbo.[User]
SET password_hash =
    'AQAAAAIAAYagAAAAEI+Z5q3KyM1uJk2TXgL9ZKktdhcBmyE+V/HrALln/lSINYRwylqQeiBAQQiFEuZEAg=='
WHERE username = 'member02';
GO
