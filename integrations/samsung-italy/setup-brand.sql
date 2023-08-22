-- sit/uat
-- SET @emailChaser1 = 120;
-- SET @emailChaser2 = 180;
-- SET @cutoff = 420;

-- prod
-- SET @emailChaser1 = 28800;
-- SET @emailChaser2 = 172800;
-- SET @cutoff = 2592000;

SELECT @samsungItalyBrandId := Id FROM `Brand` WHERE `shortcode` = 'IT' LIMIT 1;

-- auto responses

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @samsungItalyBrandId, 1, 9, 'This support ticket has been open for 30 days. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.', @cutoff, NULL, NULL),
	(UUID(), @samsungItalyBrandId, 1, 4, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.', NULL, NULL, NULL),
	(UUID(), @samsungItalyBrandId, 1, 5, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.\nIf you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @samsungItalyBrandId, 1, 6, 'Sorry, {{AdvocateFirstName}} is not currently available. We''re finding you a new support agent to get things moving', 180, NULL, NULL),
	(UUID(), @samsungItalyBrandId, 1, 8, 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @samsungItalyBrandId, 1, 10, 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @samsungItalyBrandId, 1, 11, 'Sorry, {{AdvocateFirstName}} is not currently available. We''re finding you a new support agent to get things moving', 180, NULL, NULL),
	(UUID(), @samsungItalyBrandId, 1, 13, 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL);

-- chat resumed from notification

SET @chatActionId = UUID();

INSERT INTO `chataction` (`Id`, `Type`, `IsBlocking`)
VALUES
	(@chatActionId, 1, 1);

INSERT INTO `chatactionoption` (`Id`, `ChatActionId`, `Label`, `Value`, `IsSuggested`)
VALUES
	(UUID(), @chatActionId, 'Yes', 'True', 1),
	(UUID(), @chatActionId, 'No', 'False', 0);	

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @samsungItalyBrandId, 1, 12, 'Welcome back to the chat {{CustomerFirstName}}.\n    Please help us improve.\n    Is your issues resolved?', NULL, @chatActionId, 'Customer');	

-- returning customer flow

SET @chatActionId = UUID();

INSERT INTO `chataction` (`Id`, `Type`, `IsBlocking`)
VALUES
	(@chatActionId, 1, 1);

INSERT INTO `chatactionoption` (`Id`, `ChatActionId`, `Label`, `Value`, `IsSuggested`)
VALUES
	(UUID(), @chatActionId, 'Yes', 'True', 1),
	(UUID(), @chatActionId, 'No', 'False', 0);	

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @samsungItalyBrandId, 1, 7, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.\n    Please help us improve.\n    Is your issues resolved?', NULL, @chatActionId, 'Customer');
