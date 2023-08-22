SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @parentId := Id FROM `tag` WHERE `BrandId` = @amsId AND `Name` = 'upsell-only'  LIMIT 1;

UPDATE `tag` SET `DiagnosisEnabled` = 0 WHERE `ParentTagId` = @parentId;