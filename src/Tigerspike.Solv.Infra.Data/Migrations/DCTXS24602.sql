SELECT @brandId := Id FROM solv.brand where ShortCode = 'SF';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`,`Priority`,`AuthorUserType`)
VALUES
	(UUID(), @brandId, 1, 9, 'This support ticket has been open for 1 day. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.', 86400, NULL, NULL,0,NULL);