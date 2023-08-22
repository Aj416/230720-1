SELECT @neato := Id FROM `Brand` WHERE `name` = 'Neato' limit 1;
SELECT @sectionProductProcessKnowledge := Id FROM `InductionSection` WHERE `name` = 'Brand Identity and values' AND BrandId = @neato;

UPDATE `InductionSectionItem` SET Source = 'https://neatoexplained.com/'
WHERE `Name` = 'About Us' AND SectionId = @sectionProductProcessKnowledge;