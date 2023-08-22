SELECT @neato := Id FROM `Brand` WHERE `name` = 'Neato' LIMIT 1;

INSERT INTO `WhitelistPhrase` (BrandId, Phrase)
SELECT * FROM (
	SELECT @neato, 'privacy@neatorobotics.com'
) content
WHERE @neato IS NOT NULL;

