UPDATE brand
SET SuperSolversEnabled = 1
WHERE `Name` NOT IN ('Neato');

UPDATE ticket SET `Level` = 3 WHERE `Status` < 4 AND `EscalationReason` IS NOT NULL;

UPDATE ticket t
	INNER JOIN brand b ON t.BrandId = b.Id
SET t.`Level` = 3
WHERE b.Name = 'Neato'
	AND `EscalationReason` IS NOT NULL;