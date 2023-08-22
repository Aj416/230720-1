SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;
SELECT @identityAndValues := Id FROM `InductionSection` WHERE `name` = 'Identity and values (1 hr to complete)' AND BrandId = @hpBrandId;

UPDATE `InductionSectionItem` SET `Source` = 'https://assets.solvnow.com/hp/Getting+Started+with+Solv+X+HP.pdf' , `Name` = 'Getting Started with Solv X HP'
WHERE `Name` = 'Brand rules and key info' AND SectionId = @identityAndValues;

UPDATE `InductionSectionItem` SET `Source` = 'https://assets.solvnow.com/hp/HP_SupportWeb+Search+Training[3]++-++Read-Only+(1).pdf' , `Name` = 'How to Navigate HP Website'
WHERE `Name` = 'Frequently Asked Questions' AND SectionId = @identityAndValues;

UPDATE `InductionSectionItem` SET `Source` = 'https://assets.solvnow.com/hp/How+to+communicate+with+HP+customers+(1).pdf' 
WHERE `Name` = 'How to communicate with HP customers' AND SectionId = @identityAndValues;