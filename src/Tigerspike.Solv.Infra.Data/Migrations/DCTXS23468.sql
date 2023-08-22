SELECT @brandId := Id FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @parentTagId := Id FROM `tag` WHERE `BrandId` = @brandId AND `Name` = 'upsell-only' LIMIT 1;

UPDATE `tag`
SET `Action` = 0, `DiagnosisEnabled` = 0
WHERE `BrandId` = @brandId AND `Name` = 'upsell-only';

UPDATE `tag`
SET `Action` = 0
WHERE `ParentTagId` = @parentTagId;