SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;

update solv.brand set SposEmail = 'hpsaindiacp@hp.com' where Id = @hpBrandId;