SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Samsung Australia' LIMIT 1;

Update solv.apikey set `Key` = 'samsung-australia-1' , `ApplicationId` = 'samsung-australia-1' 
where BrandId = @brandId;