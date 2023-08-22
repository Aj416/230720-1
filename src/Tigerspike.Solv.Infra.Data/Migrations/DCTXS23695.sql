SELECT @samsungBrandId := Id FROM `Brand` WHERE `Name` = 'Samsung Italy' LIMIT 1;

DELETE FROM solv.brandadvocateresponseconfig where BrandId = @samsungBrandId and `type` in (4,5,6,7,8);