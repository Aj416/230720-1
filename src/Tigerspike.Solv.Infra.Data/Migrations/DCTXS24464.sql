-- probingform

SET @probingFormId = UUID();

INSERT INTO `probingform` VALUES (@probingFormId, 'Help us connect you to the right support team by answering a few questions');

SET @q1 = UUID();
SET @q2 = UUID();

INSERT INTO `probingquestion` (`Id`, `ProbingFormId`, `Text`, `Code`, `Description`, `Order`)
VALUES
	(@q1, @probingFormId, 'Do you have an Open Case Number or is this about a Repair Status inquiry?', 'CaseNumber', '## Do you have an open case number?\n\nYour case number is a 10 digit number that begins with 5xxxxxxxxx', 1),
	(@q2, @probingFormId, 'Is your device physically damaged or not powering up?', 'DamagedDevice', 'Examples of physical damage could include cracked screen, damaged hinges, water/spillage damage, missing keys etc.', 2);

INSERT INTO `probingquestionoption` (`Id`, `QuestionId`, `Text`, `Action`, `Order`)
VALUES
	(UUID(), @q1, 'Yes', 0, 1),
	(UUID(), @q1, 'No', NULL, 2),
	(UUID(), @q2, 'Yes', 0, 1),
	(UUID(), @q2, 'No', NULL, 2);

UPDATE `brand` SET ProbingFormId = @probingFormId WHERE `Name` = 'HP AMS Print';


-- Auto Response
SELECT @ChatActionId := Id FROM solv.chataction where Type = 1;
SELECT @BrandId := Id FROM `Brand` WHERE `Name` = 'HP AMS Print' LIMIT 1;

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`,`Priority`,`AuthorUserType`)
VALUES
	(UUID(), @BrandId, 1, 9, 'This support ticket has been open for 30 days. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.', 180, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 2, 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.', NULL, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 4, 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it.', NULL, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 5, 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it. If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.', NULL, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 7, 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.\n\nPlease help us improve.\n\nIs your issue resolved?', NULL, @ChatActionId, 'Customer',0,NULL),
	(UUID(), @BrandId, 1, 8, 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 10, 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 11, 'Sorry, {{AdvocateFirstName}} is not currently available. We''re finding you a new support agent to get things moving', 180, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 13, 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.', NULL, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 3, 'Thank you for your patience. Support is quite busy at present, we are prioritising your message and a support specialist will be with you shortly', 60, NULL, NULL,0,NULL),
	(UUID(), @BrandId, 1, 22, 'Thanks for contacting support today and submitting a rating. For additional assistance with your HP products & services please visit http://www.support.hp.com', NULL, NULL, NULL,100,5);

-- brandadvocateresponseconfig Abandon count
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`)
    SELECT UUID(), Id, 1, 16, NULL, NULL, NULL, 'Customer', 2, NULL, NULL, 100 FROM `Brand` WHERE `Name` = 'HP AMS Print'; -- abandoned count

-- email chasers

INSERT INTO `brandnotificationconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `DeliverAfterSeconds`, `Subject`, `Header`, `Body`)
VALUES
	(UUID(), @BrandId, 1, 0, 28800, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', '{{AdvocateFirstName}} has marked your support ticket as solved', 'Please note, if we do not hear from you within the next <b>3 days</b>, your support ticket will be closed'),
	(UUID(), @BrandId, 1, 0, 172800, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', 'Your support ticket will be closed in the next 24 hours', 'Please note, if we do not hear from you within the next <b>1 day</b>, your support ticket will be closed');