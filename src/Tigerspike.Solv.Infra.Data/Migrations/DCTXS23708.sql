SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `brand`
SET `ValidTransferEnabled` = 1
WHERE `Id` IN (@indId, @amsId);