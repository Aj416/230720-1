SELECT @hpAmsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within 1 minute we''ll find someone else who can help.'
WHERE `Type` = 10 AND `BrandId` = @hpAmsId;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome back {{CustomerFirstName}}.
    Please help us improve.
    Is your issues resolved?'
WHERE `Type` = 12 AND `BrandId` = @hpAmsId;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'The wait time is a little longer than expected. We appreciate your patience and support will start shortly.'
WHERE `Type` IN (6,11) AND `BrandId` = @hpAmsId;