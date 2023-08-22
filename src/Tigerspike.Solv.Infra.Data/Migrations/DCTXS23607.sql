-- get brands ids
SELECT @hpIndBrandId := Id FROM `Brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @hpAmsBrandId := Id FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @neatoBrandId := Id FROM `Brand` WHERE `Name` = 'Neato' LIMIT 1;

-- remove thank you copy for brands, that are using default value
DELETE FROM `brandadvocateresponseconfig`
WHERE `BrandId` NOT IN (@hpIndBrandId, @hpAmsBrandId) AND `Type` = 14;

-- remove cut-off copy for brands
DELETE FROM `brandadvocateresponseconfig` WHERE `Type` IN (9);

-- provide default copies for all brands
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`)
VALUES
    (UUID(), NULL, 1, 9, 'This support ticket has been open for 30 days. We''ll close this now but, if you need us again, please submit a new query and our support team will be very pleased to assist you.', 2592000, NULL, 'Customer', NULL, NULL, NULL, 0),
	(UUID(), NULL, 1, 14, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, 'Customer', NULL, NULL, NULL, 0),
	(UUID(), NULL, 1, 15, 'Unfortunately, {{AdvocateFirstName}} was unable to resolve your issue in full. Please stay connected as we get some additional assistance to help close it out. We appreciate your patience.', NULL, NULL, 'Customer', NULL, NULL, NULL, 0),
	(UUID(), NULL, 1, 15, 'The wait time is a little longer than expected. We appreciate your patience and support will start shortly.', NULL, NULL, 'Customer', NULL, NULL, 1, 1),
	(UUID(), NULL, 1, 17, 'It looks like this issue needs next level assistance. We are transferring your case to our specialist support team to continue your support. Please stay connected, we will connect you very soon. Thanks for your patience.', NULL, NULL, 'Customer', NULL, NULL, NULL, 0),
	(UUID(), NULL, 1, 17, 'Thank you for your patience. Support is quite busy at present; we are prioritizing your message and elevating to a higher level support specialist. They will be with you shortly.', NULL, NULL, 'Customer', NULL, 1, NULL, 1);

-- create escalation configs
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`)
    SELECT UUID(), Id, 1, 16, NULL, NULL, NULL, 'Customer', 2, NULL, NULL, 100 FROM `Brand` WHERE `ShortCode` = 'IND'; -- abandoned count

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`)
    SELECT UUID(), Id, 1, 16, NULL, NULL, NULL, 'Customer', 2, NULL, NULL, 100 FROM `Brand` WHERE `ShortCode` = 'AMS'; -- abandoned count

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`)
    SELECT UUID(), Id, 1, 16, NULL, NULL, NULL, 'Customer', 2, NULL, NULL, 100 FROM `Brand` WHERE `Name` = 'Neato'; -- abandoned count

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`)
    SELECT UUID(), Id, 1, 18, NULL, 180, NULL, NULL, NULL, NULL, NULL, 100 FROM `Brand` WHERE `ShortCode` = 'AMS'; -- timeout

INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`)
    SELECT UUID(), Id, 1, 18, NULL, 28800, NULL, NULL, NULL, NULL, NULL, 100 FROM `Brand` WHERE `Name` = 'Neato'; -- timeout


UPDATE `brandadvocateresponseconfig` SET
  `Content`= 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.'
WHERE `Type` = 4;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.
If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.'
WHERE `Type` = 5;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= NULL
WHERE `Type` = 6;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.
Please help us improve.
Is your issues resolved?'
WHERE `Type` = 7;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= 'If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.'
WHERE `Type` = 8;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.'
WHERE `Type` = 10;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= NULL
WHERE `Type` = 11;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= 'Welcome back {{CustomerFirstName}}.
Please help us improve.
Is your issues resolved?'
WHERE `Type` = 12;

UPDATE `brandadvocateresponseconfig` SET
  `Content`= 'If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.'
WHERE `Type` = 13;