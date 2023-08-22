SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;

UPDATE `brand` SET `WaitMinutesToClose` = 4320 WHERE Id = @hpBrandId