SET SQL_SAFE_UPDATES = 0;
UPDATE ticket SET Ready = 1; -- assuming all previous created tickets on the platform was created in Ready state (as we did not have this feature before)
SET SQL_SAFE_UPDATES = 1;