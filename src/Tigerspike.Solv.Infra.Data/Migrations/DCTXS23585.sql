INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
   SELECT UUID(), b.Id, 1, 14, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, 'Customer'
   FROM brand b;

SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Thanks for contacting support today and submitting a rating. For additional assistance with your HP products & services please visit http://www.support.hp.com'
WHERE `Type` = 14 AND `BrandId` IN (@amsId, @indId);