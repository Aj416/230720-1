SELECT @hpAMSBrandId := Id FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it. \nIf you don''t receive a reply from {{AdvocateFirstName}} within 1 minute we''ll find someone else who can help.'
WHERE `Type` = 5 AND `BrandId` = @hpAMSBrandId;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 1 minute we''ll find someone else who can help.'
WHERE `Type` = 10 AND `BrandId` = @hpAMSBrandId;

UPDATE `brandadvocateresponseconfig`
SET `DelayInSeconds` = 60
WHERE `Type` = 6 AND `BrandId` = @hpAMSBrandId;

UPDATE `brandadvocateresponseconfig`
SET `DelayInSeconds` = 60
WHERE `Type` = 11 AND `BrandId` = @hpAMSBrandId;

SELECT @hpINDBrandId := Id FROM `Brand` WHERE `ShortCode` = 'IND' LIMIT 1;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}, you already have a chat open with us so we''ve taken you straight to it. \nIf you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.'
WHERE `Type` = 5 AND `BrandId` = @hpINDBrandId;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.'
WHERE `Type` = 10 AND `BrandId` = @hpINDBrandId;

UPDATE `brandadvocateresponseconfig`
SET `DelayInSeconds` = 180
WHERE `Type` = 6 AND `BrandId` = @hpINDBrandId;

UPDATE `brandadvocateresponseconfig`
SET `DelayInSeconds` = 180
WHERE `Type` = 11 AND `BrandId` = @hpINDBrandId;