UPDATE `Tag` SET Enabled = 1;

SELECT @hpIndia := Id FROM `Brand` WHERE `Name` = 'HP India' LIMIT 1;
SELECT @hpAmericas := Id FROM `Brand` WHERE `Name` = 'HP Americas' LIMIT 1;

UPDATE `Brand` SET SubTagEnabled = 1 WHERE Id = @hpIndia;
UPDATE `Brand` SET SubTagEnabled = 1 WHERE Id = @hpAmericas;

SELECT @hardwareHpIndiaTag := Id FROM `Tag` WHERE `Name` = 'hardware' AND `BrandId` = @hpIndia LIMIT 1;
SELECT @softwareHpIndiaTag := Id FROM `Tag` WHERE `Name` = 'software' AND `BrandId` = @hpIndia LIMIT 1;
SELECT @upsellonlyHpIndiaTag := Id FROM `Tag` WHERE `Name` = 'upsell-only' AND `BrandId` = @hpIndia LIMIT 1;

SET @tag1 = UUID();
SET @tag2 = UUID();
SET @tag3 = UUID();
SET @tag4 = UUID();
SET @tag5 = UUID();
SET @tag6 = UUID();
SET @tag7 = UUID();
SET @tag8 = UUID();
SET @tag9 = UUID();
SET @tag10 = UUID();
SET @tag11 = UUID();
SET @tag12 = UUID();
SET @tag13 = UUID();

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag1, @hpIndia, "CDAX: On-site raised", 0, 1, 2, @hardwareHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag2, @hpIndia, "CDAX: Other", 0, 1, 2, @hardwareHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag3, @hpIndia, "Troubleshooting fixed", null, 1, null, @softwareHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag4, @hpIndia, "General query", null, 1, null, @softwareHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag5, @hpIndia, "Customer disconnected", null, 1, null, @softwareHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag6, @hpIndia, "Referred to SVC or web", null, 1, null, @softwareHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag7, @hpIndia, "Pending customer", null, 1, null, @softwareHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag8, @hpIndia, "Warranty extension", null, 1, null, @upsellonlyHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag9, @hpIndia, "Care pack", null, 1, null, @upsellonlyHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag10, @hpIndia, "Accessory", null, 1, null, @upsellonlyHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag11, @hpIndia, "Notebook", null, 1, null, @upsellonlyHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag12, @hpIndia, "Desktop", null, 1, null, @upsellonlyHpIndiaTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag13, @hpIndia, "Printer", null, 1, null, @upsellonlyHpIndiaTag);

SELECT @hardwareHpAmericasTag := Id FROM `Tag` WHERE `Name` = 'hardware' AND `BrandId` = @hpAmericas LIMIT 1;
SELECT @softwareHpAmericasTag := Id FROM `Tag` WHERE `Name` = 'software' AND `BrandId` = @hpAmericas LIMIT 1;
SELECT @upsellonlyHpAmericasTag := Id FROM `Tag` WHERE `Name` = 'upsell-only' AND `BrandId` = @hpAmericas LIMIT 1;

SET @tag14 = UUID();
SET @tag15 = UUID();
SET @tag16 = UUID();
SET @tag17 = UUID();
SET @tag18 = UUID();
SET @tag19 = UUID();
SET @tag20 = UUID();
SET @tag21 = UUID();
SET @tag22 = UUID();
SET @tag23 = UUID();
SET @tag24 = UUID();
SET @tag25 = UUID();
SET @tag26 = UUID();

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag14, @hpAmericas, "CDAX: On-site raised", 0, 1, 2, @hardwareHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag15, @hpAmericas, "CDAX: Other", 0, 1, 2, @hardwareHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag16, @hpAmericas, "Troubleshooting fixed", null, 1, null, @softwareHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag17, @hpAmericas, "General query", null, 1, null, @softwareHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag18, @hpAmericas, "Customer disconnected", null, 1, null, @softwareHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag19, @hpAmericas, "Referred to SVC or web", null, 1, null, @softwareHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag20, @hpAmericas, "Pending customer", null, 1, null, @softwareHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag21, @hpAmericas, "Warranty extension", null, 1, null, @upsellonlyHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag22, @hpAmericas, "Care pack", null, 1, null, @upsellonlyHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag23, @hpAmericas, "Accessory", null, 1, null, @upsellonlyHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag24, @hpAmericas, "Notebook", null, 1, null, @upsellonlyHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag25, @hpAmericas, "Desktop", null, 1, null, @upsellonlyHpAmericasTag);

INSERT INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`)
VALUES
(@tag26, @hpAmericas, "Printer", null, 1, null, @upsellonlyHpAmericasTag);