SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;
SELECT @lazadaBrandId := Id FROM `Brand` WHERE `name` = 'Lazada' limit 1;
UPDATE `brand` 
SET `TicketsImportEnabled` = 1
WHERE `Id` IN (@hpBrandId, @lazadaBrandId);