SELECT @neato := Id FROM `Brand` WHERE `name` = 'Neato' limit 1;
SELECT @sectionAdditionalSupportLinks := Id FROM `InductionSection` WHERE `name` = 'Additional support links' AND BrandId = @neato;
SELECT @sectionProductProcessKnowledge := Id FROM `InductionSection` WHERE `name` = 'Product/Process knowledge' AND BrandId = @neato;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Ticket escalation?', 'https://assets.solvnow.com/neato/When+and+how+to+transfer+tickets.pdf', 1, @sectionAdditionalSupportLinks AS SectionId UNION
	SELECT UUID(), 'Delete my account queries', 'https://assets.solvnow.com/neato/Solv+Neato+Account+Deletion+requests.pdf', 1, @sectionAdditionalSupportLinks UNION
	SELECT UUID(), 'Neato chat training', 'https://assets.solvnow.com/neato/Solv+Neato+chat+training.pdf', 1, @sectionAdditionalSupportLinks
) itemsAdditionalSupportLinks
WHERE SectionId IS NOT NULL;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Troubleshooting connectivity issues', 'https://assets.solvnow.com/neato/Troubleshooting+-+Connectivity+Issues+Solv+Neato.pdf', 1, @sectionProductProcessKnowledge AS SectionId
) itemsAdditionalSupportLinks
WHERE SectionId IS NOT NULL;