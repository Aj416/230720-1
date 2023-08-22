SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
update solv.brand set CreateTicketInstructions = 'If you would like to learn more about HP''s Privacy please read the [HP Privacy Statement](https://www8.hp.com/us/en/privacy/privacy-central.html)' where Id in (@indId,@amsId);