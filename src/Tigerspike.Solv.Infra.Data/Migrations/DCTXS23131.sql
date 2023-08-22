SELECT @neatoId := Id FROM `brand` WHERE `Name` = 'Neato' LIMIT 1;

SELECT @obsoleteId1 := Id FROM `abandonreason` WHERE `Name` = 'No information' AND `BrandId` = @neatoId LIMIT 1;
SELECT @obsoleteId2 := Id FROM `abandonreason` WHERE `Name` = 'Accident' AND `BrandId` = @neatoId LIMIT 1;
SELECT @obsoleteId3 := Id FROM `abandonreason` WHERE `Name` = 'Need more info' AND `BrandId` = @neatoId LIMIT 1;

UPDATE `abandonreason`
SET `IsActive` = 0
WHERE `Id` IN (@obsoleteId1, @obsoleteId2, @obsoleteId3);

SELECT @Id1 := Id FROM `abandonreason` WHERE `Name` = 'Complaint' AND `BrandId` = @neatoId LIMIT 1;
SELECT @Id2 := Id FROM `abandonreason` WHERE `Name` = 'Follow up/Repair' AND `BrandId` = @neatoId LIMIT 1;
SELECT @Id3 := Id FROM `abandonreason` WHERE `Name` = 'GDPR' AND `BrandId` = @neatoId LIMIT 1;
SELECT @Id4 := Id FROM `abandonreason` WHERE `Name` = 'Return/Refund' AND `BrandId` = @neatoId LIMIT 1;
SELECT @Id5 := Id FROM `abandonreason` WHERE `Name` = 'Follow up' AND `BrandId` = @neatoId LIMIT 1;

UPDATE `abandonreason`
SET `Action` = 0
WHERE `Id` IN (@Id1, @Id2, @Id3, @Id4, @Id5);

UPDATE `abandonreason`
SET `Name` = 'Repair required'
WHERE `Id` = @Id2;