SELECT @hpIndiaId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;

-- remove WA escalation
UPDATE brandadvocateresponseconfig
SET IsActive = 0
WHERE BrandId = @hpIndiaId AND Type = 3 AND ChatActionId IS NOT NULL;

-- add 4 minutes escalation
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`, `AuthorUserType`)
VALUES
	(UUID(), @hpIndiaId, 1, 18, NULL, 240, NULL, NULL, NULL, NULL, NULL, 100, NULL);
