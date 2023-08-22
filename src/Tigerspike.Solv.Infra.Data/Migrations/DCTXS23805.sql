-- chat actions
SELECT @csatChatActionId := Id FROM `chataction` WHERE `Type` = 2 LIMIT 1;
SELECT @npsChatActionId := Id FROM `chataction` WHERE `Type` = 3 LIMIT 1;

-- delete old thank you messages
DELETE FROM `brandadvocateresponseconfig` WHERE `Type` IN (14, 19, 20, 21);

-- set default flow
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
VALUES
	(UUID(), NULL, 1, 19, 'How would you rate this support?', NULL, @csatChatActionId, 'Customer', 0, 1),
	(UUID(), NULL, 1, 21, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, 'Customer', 0, 5);

-- set flow for hp india
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 19, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @npsChatActionId, 'Customer', 100, 1 FROM `Brand` WHERE `ShortCode` = 'IND';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 20, 'How would you rate this support?', NULL, @csatChatActionId, 'Customer', 100, 1 FROM `Brand` WHERE `ShortCode` = 'IND';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 21, 'Thanks for contacting support today and submitting a rating. For additional assistance with your HP products & services please visit http://www.support.hp.com', NULL, NULL, 'Customer', 100, 5 FROM `Brand` WHERE `ShortCode` = 'IND';

-- set flow for hp americas
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 19, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @npsChatActionId, 'Customer', 100, 1 FROM `Brand` WHERE `ShortCode` = 'AMS';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 20, 'How would you rate this support?', NULL, @csatChatActionId, 'Customer', 100, 1 FROM `Brand` WHERE `ShortCode` = 'AMS';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 21, 'Thanks for contacting support today and submitting a rating. For additional assistance with your HP products & services please visit http://www.support.hp.com', NULL, NULL, 'Customer', 100, 5 FROM `Brand` WHERE `ShortCode` = 'AMS';



-- set flow for company one
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 19, 'How would you rate this support?', NULL, @csatChatActionId, 'Customer', 100, 1 FROM `Brand` WHERE `Name` = 'CompanyOne';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 21, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @npsChatActionId, 'Customer', 100, 1 FROM `Brand` WHERE `Name` = 'CompanyOne';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 20, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, 'Customer', 100, 5 FROM `Brand` WHERE `Name` = 'CompanyOne';