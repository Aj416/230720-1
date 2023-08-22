SELECT @otro := Id FROM `Brand` WHERE `name` = 'Otro' LIMIT 1;

INSERT INTO `WhitelistPhrase` (BrandId, Phrase)
SELECT * FROM (
	SELECT @otro, 'careers@otro.com' UNION
	SELECT @otro, 'partnerships@otro.com' UNION
	SELECT @otro, 'info@otro.com' UNION
	SELECT @otro, 'privacy@orto.com'
) content
WHERE @otro IS NOT NULL;

