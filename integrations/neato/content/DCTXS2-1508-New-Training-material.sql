SELECT @neato := Id FROM `Brand` WHERE `name` = 'Neato' limit 1;
SELECT @sectionProductProcessKnowledge := Id FROM `InductionSection` WHERE `name` = 'PRODUCT/PROCESS KNOWLEDGE' AND BrandId = @neato;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Neato Warranty Guidance', 'https://assets.solvnow.com/neato/Neato+Warranty+Guidance.pdf', 1, @sectionProductProcessKnowledge AS SectionId
) itemsProductProcessKnowledge
WHERE SectionId IS NOT NULL;

SELECT @sectionAdditionalSupportLinks := Id FROM `InductionSection` WHERE `name` = 'ADDITIONAL SUPPORT LINKS' AND BrandId = @neato;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Floor Plan Issues', 'https://assets.solvnow.com/neato/Floor+Plan+Issues.pdf', 1, @sectionAdditionalSupportLinks AS SectionId
) itemsAdditionalSupportLinks
WHERE SectionId IS NOT NULL;