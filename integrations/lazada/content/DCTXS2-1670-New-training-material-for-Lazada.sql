SELECT @lazada := Id FROM `Brand` WHERE `name` = 'Lazada' limit 1;
SELECT @sectionSolverknowledge := Id FROM `InductionSection` WHERE `name` = 'SOLVER KNOWLEDGE' AND BrandId = @lazada;

UPDATE `InductionSectionItem` set `Name` = 'Suggested responses and helpful links' where `Name` = 'Chat Macros' and SectionId = @sectionSolverknowledge;

UPDATE `InductionSectionItem` set `Name` = 'Customer follow up forms', 
`Source` = 'https://assets.solvnow.com/lazada/Customer+Follow+Up+Forms.pdf'
where `Name` = 'Lazada Buyer Survey Request forms' and SectionId = @sectionSolverknowledge;

DELETE FROM `InductionSectionItem` WHERE `Name` = 'Lazada Steps to SetUp Guide' and SectionId = @sectionSolverknowledge;

UPDATE `InductionSectionItem` set `Name` = 'Peers Payment Cycle' where `Name` = 'Lazada Payment Cycle' and SectionId = @sectionSolverknowledge;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Cybersecurity training', 'https://assets.solvnow.com/lazada/Cybersecurity+Training.pdf', 1, @sectionSolverknowledge AS SectionId
) itemsProductKnowledge
WHERE SectionId IS NOT NULL;

SELECT @sectionProductKnowledge := Id FROM `InductionSection` WHERE `name` = 'PRODUCT KNOWLEDGE' AND BrandId = @lazada;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Returns and Refunds Further Info', 'https://assets.solvnow.com/lazada/Returns+and+Refunds+Further+Info.pdf', 1, @sectionProductKnowledge AS SectionId UNION
	SELECT UUID(), 'Where is my Order Further Info', 'https://assets.solvnow.com/lazada/Where+is+my+Order+Order+Further+Info.pdf', 1, @sectionProductKnowledge
) itemsProductKnowledge
WHERE SectionId IS NOT NULL;