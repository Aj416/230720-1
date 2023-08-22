SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Samsung' LIMIT 1;

update solv.brand set `Name` = 'Samsung Australia' , `ShortCode` = 'AU' where id = @brandId;
