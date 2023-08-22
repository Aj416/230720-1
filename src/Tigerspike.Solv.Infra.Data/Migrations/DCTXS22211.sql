SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;

UPDATE `brandformfield` SET IsKey = 1 WHERE BrandId = @hpBrandId AND `Name` IN ('phoneNumber', 'serialNumber');

INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @hpBrandId, 1, 4, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.', NULL, NULL, NULL),
	(UUID(), @hpBrandId, 1, 5, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.\nIf you don''t receive a reply from {{AdvocateFirstName}} within 1 mintue we''ll find someone else who can help.', NULL, NULL, NULL);