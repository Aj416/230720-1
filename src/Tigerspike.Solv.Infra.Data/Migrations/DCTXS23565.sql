SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @obsoleteId := Id, @parentId := ParentTagId FROM `tag` WHERE `BrandId` = @amsId AND `Name` = 'Referred to SVC or web'  LIMIT 1;

UPDATE `tag` SET `Enabled` = 0 WHERE `Id` = @obsoleteId;
INSERT IGNORE INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`)
VALUES
	(UUID(), @amsId, 'Referred to SVC', NULL, 1, NULL, @parentId, NULL, NULL),
	(UUID(), @amsId, 'Referred to website/self-help', NULL, 1, NULL, @parentId, NULL, NULL);
