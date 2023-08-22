SELECT @samsungItalyBrandId := Id FROM `Brand` WHERE `Name` = 'Samsung Italy' LIMIT 1;

update solv.brand set SposEnabled = 0, SposEmail = '', NpsEnabled = 0 where Id = @samsungItalyBrandId;

update solv.brandadvocateresponseconfig set IsActive = 0 where BrandId = @samsungItalyBrandId and `type` in (0,2,3);