SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Wellnest' LIMIT 1;
SELECT @chatActionId := id FROM solv.chataction where Type = 2;

update solv.brandadvocateresponseconfig set ChatActionId = @chatActionId where Type = 19 and BrandId = @brandId;

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
VALUES
	(UUID(), @brandId, 1, 22, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, 'Customer', 0, 5);