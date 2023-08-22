SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Samsung Australia' LIMIT 1;

update solv.brand set Code = 'Australia' where id = @brandId;