INSERT INTO `abandonreason` (`Id`, `Name`, `BrandId`, `IsActive`, `IsAutoAbandoned`)
	SELECT UUID(), 'Auto-abandoned', `Id`, 1, 1 FROM `brand`
;

SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;
SET @chatActionId = UUID();

INSERT INTO `chataction` (`Id`, `Type`, `IsBlocking`)
VALUES
	(@chatActionId, 1, 1);

INSERT INTO `chatactionoption` (`Id`, `ChatActionId`, `Label`, `Value`, `IsSuggested`)
VALUES
	(UUID(), @chatActionId, 'Yes', 'True', 1),
	(UUID(), @chatActionId, 'No', 'False', 0);

INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @hpBrandId, 1, 6, 'Sorry, {{AdvocateFirstName}} is not currently available. We''re finding you a new support agent to get things moving', 60, NULL, NULL),
	(UUID(), @hpBrandId, 1, 7, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.
Please help us improve.
Is your issues resolved?', NULL, @chatActionId, 'Customer'),
	(UUID(), @hpBrandId, 1, 8, 'If you don''t receive a reply from {{AdvocateFirstName}} within 1 mintue we''ll find someone else who can help.', NULL, NULL, NULL);

