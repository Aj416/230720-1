SELECT @brandId := Id FROM `Brand` WHERE `ShortCode` = 'IT' LIMIT 1;

UPDATE `solv`.`brandadvocateresponseconfig`
SET
`IsActive` = 0
WHERE `BrandId` = @brandId AND `Type` IN (0, 2);