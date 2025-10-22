START TRANSACTION;
DROP TABLE user_management."Users";

DROP TABLE user_management."Roles";

DELETE
FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250414220359_BigBang';

COMMIT;

