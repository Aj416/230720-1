SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;

INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @hpBrandId, 1, 9, 'This chat has been open for 30 days. We''ll close this enquiry now but if you need us again please submit a new query.', 2592000, NULL, NULL);
