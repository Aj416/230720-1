SELECT @brandId := Id FROM solv.brand where ShortCode = 'SF';

-- auto responses
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @brandId, 1, 0, 'Thank you for reaching out to SmartFriend! SmartFriend support is currently available via voice by calling #1-866-211-5207 or for all other HP Support please contact #1-800-474-6836.', NULL, NULL, NULL);