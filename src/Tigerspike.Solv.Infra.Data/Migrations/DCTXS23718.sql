SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @obsoleteId := Id, @parentId := ParentTagId FROM `tag` WHERE `BrandId` = @indId AND `Name` = 'Referred to SVC or web'  LIMIT 1;

UPDATE `tag` SET `Enabled` = 0 WHERE `Id` = @obsoleteId;
INSERT IGNORE INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`)
VALUES
	(UUID(), @indId, 'Referred to SVC', NULL, 1, NULL, @parentId, NULL, NULL),
	(UUID(), @indId, 'Referred to website/self-help', NULL, 1, NULL, @parentId, NULL, NULL);
