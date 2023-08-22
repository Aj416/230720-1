SELECT @lazada := Id FROM `Brand` WHERE `name` = 'Lazada' limit 1;
SELECT @sectionSolverKnowledge := Id FROM `InductionSection` WHERE `name` = 'Solver knowledge' AND BrandId = @lazada;

INSERT INTO `InductionSectionItem` (Id, `Name`, Source, Enabled, SectionId)
SELECT * FROM (
	SELECT UUID(), 'Chat Macros', 'https://assets.solvnow.com/lazada/Chat+Macros.xlsx', 1, @sectionSolverKnowledge AS SectionId UNION
	SELECT UUID(), 'Lazada Buyer Survey Request Forms', 'https://assets.solvnow.com/lazada/LAZ+Buyer+Survey++Request+Forms.pdf', 1, @sectionSolverKnowledge UNION
	SELECT UUID(), 'Lazada Steps to SetUp Guide', 'https://assets.solvnow.com/lazada/Lazada+Steps+to+SetUp+Guide.pdf', 1, @sectionSolverKnowledge UNION
	SELECT UUID(), 'Lazada Payment Cycle', 'https://assets.solvnow.com/lazada/Lazada+Payment+Cycle.pdf', 1, @sectionSolverKnowledge UNION
	SELECT UUID(), 'Lazada Chat Category Tagging Guide', 'https://assets.solvnow.com/lazada/LAZ+Chat+Category+Tagging+Guide.pdf', 1, @sectionSolverKnowledge UNION
	SELECT UUID(), 'Delivery Adjustment Checker', 'https://assets.solvnow.com/lazada/Delivery+Adjustment+Checker.pdf', 1, @sectionSolverKnowledge
) itemsSolverKnowledge
WHERE SectionId IS NOT NULL;
