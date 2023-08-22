-- sit/uat
-- SET @emailChaser1 = 120;
-- SET @emailChaser2 = 180;
-- SET @cutoff = 420;

-- prod
-- SET @emailChaser1 = 28800;
-- SET @emailChaser2 = 172800;
-- SET @cutoff = 2592000;

SELECT @hpAmsBrandId := Id FROM `Brand` WHERE `shortcode` = 'AMS' LIMIT 1;


-- form setup/metadata

INSERT INTO `brandformfield` (`Id`, `Name`, `Title`, `TypeId`, `IsRequired`, `Validation`, `DefaultValue`, `Order`, `CreatedDate`, `ModifiedDate`, `BrandId`, `Options`, `IsKey`, `AccessLevel`)
VALUES
	(UUID(), 'phoneNumber', 'Phone Number', 1, 1, 'required,phone', '', 2, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @hpAmsBrandId, NULL, 1, 3),
	(UUID(), 'serialNumber', 'Serial Number', 1, 1, 'required', '', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @hpAmsBrandId, NULL, 1, 3),
	(UUID(), 'productNumberOrModelName', 'Product Number or Model Name', 1, 1, 'required', '', 3, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @hpAmsBrandId, NULL, 0, 2);

-- email chasers

INSERT INTO `brandnotificationconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `DeliverAfterSeconds`, `Subject`, `Header`, `Body`)
VALUES
	(UUID(), @hpAmsBrandId, 1, 0, @emailChaser1, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', '{{AdvocateFirstName}} has marked your support ticket as solved', 'Please note, if we do not hear from you within the next <b>3 days</b>, your support ticket will be closed'),
	(UUID(), @hpAmsBrandId, 1, 0, @emailChaser2, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', 'Your support ticket will be closed in the next 24 hours', 'Please note, if we do not hear from you within the next <b>1 day</b>, your support ticket will be closed');

-- auto responses

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @hpAmsBrandId, 1, 9, 'This support ticket has been open for 30 days. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.', @cutoff, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 0, 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.', NULL, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 2, 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.', NULL, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 4, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.', NULL, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 5, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.\nIf you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 6, 'Sorry, {{AdvocateFirstName}} is not currently available. We''re finding you a new support agent to get things moving', 180, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 8, 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 10, 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 11, 'Sorry, {{AdvocateFirstName}} is not currently available. We''re finding you a new support agent to get things moving', 180, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 13, 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL),
	(UUID(), @hpAmsBrandId, 1, 3, 'Thank you for your patience. Support is quite busy at present, we are prioritising your message and a support specialist will be with you shortly', 60, NULL, NULL);

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
	(UUID(), @hpAmsBrandId, 1, 12, 'Welcome back to the chat {{CustomerFirstName}}.\n    Please help us improve.\n    Is your issues resolved?', NULL, @chatActionId, 'Customer');	

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
	(UUID(), @hpAmsBrandId, 1, 7, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.\n    Please help us improve.\n    Is your issues resolved?', NULL, @chatActionId, 'Customer');


-- escalation configs
INSERT INTO `ticketsource` (`Name`)
VALUES
	('HP_PC'),
	('HP_MAC'),
	('HP_PRINT');

SELECT @hpPcSourceId := Id FROM `ticketsource` WHERE `Name` = 'HP_PC' LIMIT 1;
SELECT @hpMacSourceId := Id FROM `ticketsource` WHERE `Name` = 'HP_MAC' LIMIT 1;
SELECT @hpPrintSourceId := Id FROM `ticketsource` WHERE `Name` = 'HP_PRINT' LIMIT 1;

INSERT INTO `ticketescalationconfig` (`Id`, `BrandId`, `TicketSourceId`, `OpenTimeInSeconds`, `RejectionCount`, `AbandonedCount`, `CustomerMessage`)
VALUES
	(UUID(), @hpAmsBrandId, NULL, 180, 3, 3, 'Your issue is being escalated and reallocated. Someone will be with you soon.'),
	(UUID(), @hpAmsBrandId, @hpPcSourceId, 180, 3, 3, 'Your issue is being escalated and reallocated. Someone will be with you soon.'),
	(UUID(), @hpAmsBrandId, @hpMacSourceId, 180, 3, 3, 'Your issue is being escalated and reallocated. Someone will be with you soon.'),
	(UUID(), @hpAmsBrandId, @hpPrintSourceId, 180, 3, 3, 'Your issue is being escalated and reallocated. Someone will be with you soon.');
