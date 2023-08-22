SELECT @BrandId := Id from `Brand` where `Name` = 'HP' limit 1;
UPDATE `Brand` SET `Color` = '#000000' WHERE `Id` = @BrandId;

SELECT @BrandId := Id from `Brand` where `Name` = 'HP SmartFriend' limit 1;
UPDATE `Brand` SET `Color` = '#0097D5' WHERE `Id` = @BrandId;

SELECT @BrandId := Id from `Brand` where `Name` = 'Lazada' limit 1;
UPDATE `Brand` SET `Color` = '#0F156D' WHERE `Id` = @BrandId;

SELECT @BrandId := Id from `Brand` where `Name` = 'Neato' limit 1;
UPDATE `Brand` SET `Color` = '#FC4C02' WHERE `Id` = @BrandId;

SELECT @BrandId := Id from `Brand` where `Name` = 'OTRO' limit 1;
UPDATE `Brand` SET `Color` = '#FF2553' WHERE `Id` = @BrandId;