-- require New status for escalation feature
UPDATE brandadvocateresponseconfig
SET StatusOnPosting = 0
WHERE Type IN (3, 18);

-- restore hp india WA escalation and remove timeout escalation
SELECT @hpIndiaId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;

-- restore regular WA escalation for HP india
UPDATE brandadvocateresponseconfig
SET IsActive = 1
WHERE BrandId = @hpIndiaId AND Type = 3 AND ChatActionId IS NOT NULL;

DELETE FROM `brandadvocateresponseconfig`
WHERE BrandId = @hpIndiaId AND Type = 18;