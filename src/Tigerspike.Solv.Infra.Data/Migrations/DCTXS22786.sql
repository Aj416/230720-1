SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;
UPDATE `brandadvocateresponseconfig` 
SET `Content` = 'This support ticket has been open for 30 days. We’ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.'
WHERE `BrandId` = @hpBrandId and `Type` = 9;