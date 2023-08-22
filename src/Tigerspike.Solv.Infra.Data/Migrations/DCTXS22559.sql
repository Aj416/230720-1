SELECT @solvSupportId := Id FROM `Brand` WHERE `name` = 'Solv Support' LIMIT 1;

SET @sectionId := UUID();

INSERT INTO `inductionsection` (`Id`, `Name`, `Enabled`, `BrandId`)
VALUES
	(@sectionId, 'Solver knowledge', 1, @solvSupportId);

INSERT INTO `inductionsectionitem` (`Id`, `Name`, `Source`, `Enabled`, `SectionId`)
VALUES
	(UUID(), 'Privacy policy', 'https://www.solvnow.com/privacy', 1, @sectionId);
