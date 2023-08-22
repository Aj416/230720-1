SELECT @dellBrandId := Id FROM `Brand` WHERE `Name` = 'Dell' LIMIT 1;

DELETE FROM solv.brandadvocateresponseconfig where BrandId = @dellBrandId and `type` in (4,5,6,7,8);