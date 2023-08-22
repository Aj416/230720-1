SELECT @amsId := Id FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @indId := Id FROM `Brand` WHERE `ShortCode` = 'IND' LIMIT 1;

-- DCTXS2-3571
UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.'
WHERE `Type` = 4; -- global change

-- DCTXS2-3571
UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.
    Please help us improve.
    Is your issues resolved?'
WHERE `Type` = 7; -- global change

-- DCTXS2-3571
UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.
    If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.'
WHERE `Type` = 5 AND `BrandId` IN (@indId); -- only hp india

-- DCTXS2-3571
UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.
    If you don''t receive a reply from {{AdvocateFirstName}} within 1 minute we''ll find someone else who can help.'
WHERE `Type` = 5 AND `BrandId` IN (@amsId); -- only hp americas

-- DCTXS2-3572
UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your support specialist today. Please give me a few moments while I review your issue. Please note, for security reasons, do NOT send credit card information via chat.'
WHERE `Type` IN (0,2) AND `BrandId` IN (@indId, @amsId); -- only hp brands

-- DCTXS2-3573
UPDATE `brandadvocateresponseconfig`
SET `Content` = 'The wait time is a little longer than expected. We appreciate your patience and support will start shortly.'
WHERE `Type` = 11; -- global change

-- DCTXS2-3576
UPDATE `brandadvocateresponseconfig`
SET `Content` = 'This support ticket has been open for 30 days. We''ll close this now but, if you need us again, please submit a new query and our support team will be very pleased to assist you.'
WHERE `Type` = 9; -- global change