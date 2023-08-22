SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @hardwareIndId := Id FROM `tag` WHERE `BrandId` = @indId AND `Name` = 'hardware' LIMIT 1;

INSERT IGNORE INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`)
VALUES
	(UUID(), @indId, 'CDAX: Status check', 0, 1, 2, @hardwareIndId, NULL, NULL);


SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @hardwareAmsId := Id FROM `tag` WHERE `BrandId` = @amsId AND `Name` = 'hardware' LIMIT 1;

INSERT IGNORE INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`)
VALUES
	(UUID(), @amsId, 'CDAX: Status check', 0, 1, 2, @hardwareAmsId, NULL, NULL);