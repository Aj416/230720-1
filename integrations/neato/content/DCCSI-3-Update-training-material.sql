SELECT @neato := Id FROM `Brand` WHERE `name` = 'Neato' limit 1;
SELECT @sectionProductProcessKnowledge := Id FROM `InductionSection` WHERE `name` = 'PRODUCT/PROCESS KNOWLEDGE' AND BrandId = @neato;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Knowledge Bank', 'https://assets.solvnow.com/neato/Neato+Knowledge+Base+V2.pdf', 1, @sectionProductProcessKnowledge AS SectionId
) itemsAdditionalSupportLinks
WHERE SectionId IS NOT NULL;

UPDATE `InductionSectionItem` SET Source = 'https://www.neatorobotics.com/gb/robot-vacuum/d-shape-series/'
WHERE `Name` = 'Product range' AND SectionId = @sectionProductProcessKnowledge;

DELETE FROM `InductionSectionItem`
WHERE `Name` = 'How to video' AND SectionId = @sectionProductProcessKnowledge; 