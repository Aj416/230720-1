SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;
SELECT @identityAndValues := Id FROM `InductionSection` WHERE `name` = 'Identity and values (1 hr to complete)' AND BrandId = @hpBrandId;

UPDATE `InductionSectionItem` SET `Source` = 'https://assets.solvnow.com/hp/Getting+Started+with+Solv+X+HP+V3++-++Read-Only.pdf'
WHERE `Name` = 'Getting Started with Solv X HP' AND SectionId = @identityAndValues;

SELECT @productProcessKnowledge := Id FROM `InductionSection` WHERE `name` = 'Product/process knowledge (2.5 hrs to complete)' AND BrandId = @hpBrandId;
INSERT INTO `inductionsectionitem` (`Id`, `Name`, `Source`, `Enabled`, `SectionId`)
VALUES (
		UUID(),
		'Probing questions guide',
		'https://assets.solvnow.com/hp/Knowledge+bytes++-++Read-Only.pdf',
		1,
		@productProcessKnowledge
	);