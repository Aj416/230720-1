SET @chatActionId = UUID();

INSERT INTO `chataction` (`Id`, `Type`, `IsBlocking`)
VALUES
	(@chatActionId, 5, 1);

INSERT INTO `chatactionoption` (`Id`, `ChatActionId`, `Label`, `Value`, `IsSuggested`)
VALUES
	(UUID(), @chatActionId, 'Submit', 'True', 1),
	(UUID(), @chatActionId, 'Skip', 'False', 0);	

SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @companyOneId := Id FROM `brand` WHERE `Name` = 'CompanyOne' LIMIT 1;

UPDATE `brandadvocateresponseconfig`
SET
`Content` = 'Help us by explaining your scores...',
`ChatActionId` = @chatActionId,
`AuthorUserType` = 1
WHERE `BrandId` IN (@indId, @amsId) AND `Type` = 21;

UPDATE `brandadvocateresponseconfig`
SET
`Content` = 'Help us by explaining your scores...',
`ChatActionId` = @chatActionId,
`AuthorUserType` = 1
WHERE `BrandId` = @companyOneId AND `Type` = 20;

UPDATE `brandadvocateresponseconfig`
SET
`Content` = 'Help us by explaining your scores...',
`ChatActionId` = @chatActionId,
`AuthorUserType` = 1
WHERE `BrandId` IS NULL AND `Type` = 21;

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 22, 'Thanks for contacting support today and submitting a rating. For additional assistance with your HP products & services please visit http://www.support.hp.com', NULL, NULL, 'Customer', 100, 5 FROM `Brand` WHERE `ShortCode` = 'IND';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 22, 'Thanks for contacting support today and submitting a rating. For additional assistance with your HP products & services please visit http://www.support.hp.com', NULL, NULL, 'Customer', 100, 5 FROM `Brand` WHERE `ShortCode` = 'AMS';

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
	SELECT UUID(), Id, 1, 22, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, 'Customer', 100, 5 FROM `Brand` WHERE `Name` = 'CompanyOne';	

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `Priority`, `AuthorUserType`)
VALUES
	(UUID(), NULL, 1, 22, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, 'Customer', 0, 5);