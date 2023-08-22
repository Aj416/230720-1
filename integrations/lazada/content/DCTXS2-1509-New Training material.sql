SELECT @lazada := Id FROM `Brand` WHERE `name` = 'Lazada' limit 1;
SELECT @sectionProductKnowledge := Id FROM `InductionSection` WHERE `name` = 'PRODUCT KNOWLEDGE' AND BrandId = @lazada;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Chat eligibility guidelines', 'https://assets.solvnow.com/lazada/Chat+eligibility+guidelines.pdf', 1, @sectionProductKnowledge AS SectionId
) itemsProductKnowledge
WHERE SectionId IS NOT NULL;