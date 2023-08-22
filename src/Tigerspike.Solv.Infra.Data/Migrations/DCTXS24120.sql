SELECT @brandId := Id FROM `Brand` WHERE `ShortCode` = 'IT' LIMIT 1;

UPDATE `solv`.`brand`
SET
`SposEmail` = NULL,
`SposEnabled` = 0
WHERE `Id` = @brandId;

UPDATE `solv`.`tag`
SET
`Enabled` = 0
WHERE `BrandId` = @brandId AND `Name` = 'upsell-only';
