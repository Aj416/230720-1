SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.
        If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.'
WHERE `Type` = 5 AND `BrandId` = @amsId;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.'
WHERE `Type` = 8 AND `BrandId` = @amsId;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.'
WHERE `Type` = 10 AND `BrandId` = @amsId;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'If you don''t receive a reply from {{AdvocateFirstName}} within 3 minutes we''ll find someone else who can help.'
WHERE `Type` = 13 AND `BrandId` = @amsId;

UPDATE `brandadvocateresponseconfig`
SET `DelayInSeconds` = 180
WHERE `Type` IN (6,11) AND `BrandId` = @amsId;
