SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `tag`
SET `Enabled` = 0
WHERE `BrandId` = @amsId AND `Name` = 'Referred to SVC';