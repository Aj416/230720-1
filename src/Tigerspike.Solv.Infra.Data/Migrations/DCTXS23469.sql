SELECT @hpAmsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `tag` SET SposNotificationEnabled = 0 WHERE Name = 'upsell-only' AND BrandId = @hpAmsId;