SELECT @dellBrandId := Id FROM `Brand` WHERE `Name` = 'Dell' LIMIT 1;

update solv.brand set SposEnabled = 0, SposEmail = '' where Id = @dellBrandId;

DELETE FROM solv.brandadvocateresponseconfig where BrandId = @dellBrandId and `type` in (0,2);