SELECT @hpBrandId :=  Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;

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
	(UUID(), @hpBrandId, 1, 10, 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 1 minute we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @hpBrandId, 1, 11, 'Sorry, {{AdvocateFirstName}} is not currently available. We''re finding you a new support agent to get things moving', 60, NULL, NULL),
	(UUID(), @hpBrandId, 1, 12, 'Welcome back to the chat {{CustomerFirstName}}.
Please help us improve.
Is your issues resolved?', NULL, @chatActionId, 'Customer'),
	(UUID(), @hpBrandId, 1, 13, 'If you don''t receive a reply from {{AdvocateFirstName}} within 1 minute we''ll find someone else who can help.', NULL, NULL, NULL);