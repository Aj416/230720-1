SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;

UPDATE `abandonreason` SET IsActive = 0 WHERE BrandId = @hpBrandId AND IsForcedEscalation = 0 AND IsBlockedAdvocate = 0 AND IsAutoAbandoned = 0;

INSERT IGNORE INTO `abandonreason` (`Id`, `Name`, `BrandId`, `IsActive`, `IsForcedEscalation`, `IsBlockedAdvocate`, `IsAutoAbandoned`, `Action`)
VALUES
	(UUID(), 'Unhappy customer/Complaint', @hpBrandId, 1, 0, 0, 0, 0),
	(UUID(), 'Customer has an existing query/case', @hpBrandId, 1, 0, 0, 0, 0),
	(UUID(), 'Warranty dispute', @hpBrandId, 1, 0, 0, 0, 0),
	(UUID(), 'I don''t have the skills/knowledge to resolve', @hpBrandId, 1, 0, 0, 0, NULL);
