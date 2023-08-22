SELECT @lazada := Id FROM `Brand` WHERE `name` = 'Lazada' limit 1;
SELECT @sectionProductKnowledge := Id FROM `InductionSection` WHERE `name` = 'System training' AND BrandId = @lazada;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Software Download Page', 'https://yida.alibaba-inc.com/o/peers#/', 1, @sectionProductKnowledge AS SectionId
) itemsProductKnowledge
WHERE SectionId IS NOT NULL;