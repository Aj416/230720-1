SELECT @hpIndBrandId := Id FROM `Brand` WHERE `Name` = 'HP India' LIMIT 1;
SELECT @hpAmsBrandId := Id FROM `Brand` WHERE `Name` = 'HP Americas' LIMIT 1;

UPDATE solv.ticketescalationconfig SET AbandonedCount = 2 where BrandId = @hpIndBrandId;
UPDATE solv.ticketescalationconfig SET AbandonedCount = 2 where BrandId = @hpAmsBrandId;