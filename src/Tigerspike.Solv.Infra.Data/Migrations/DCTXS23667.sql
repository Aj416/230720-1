SELECT @neatoId := Id FROM `brand` WHERE `Name` = 'Neato' LIMIT 1;

SELECT @obsoleteId1 := Id FROM `abandonreason` WHERE `Name` = 'Taking leave' AND `BrandId` = @neatoId LIMIT 1;

UPDATE `abandonreason`
SET `IsActive` = 0
WHERE `Id` = @obsoleteId1;