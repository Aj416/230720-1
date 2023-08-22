SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Samsung Italy' LIMIT 1;

update solv.brand set WaitMinutesToClose = 1440 where Id = @brandId;

update solv.brandadvocateresponseconfig set DelayInSeconds = 172800 where `type` = 9 and brandid = @brandId;