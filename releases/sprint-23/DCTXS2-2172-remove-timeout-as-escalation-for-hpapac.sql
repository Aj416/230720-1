SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;

UPDATE ticketescalationconfig SET OpenTimeInSeconds = NULL WHERE BrandId = @hpBrandId;