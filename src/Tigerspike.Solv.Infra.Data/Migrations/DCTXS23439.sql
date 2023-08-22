SELECT @hpAmsId := Id FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `ticketescalationconfig` SET `OpenTimeInSeconds` = 180 WHERE `BrandId` = @hpAmsId;
