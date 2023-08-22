Update `abandonreason` set IsActive = 0 where `Name` = 'Don''t like the price';
Update `abandonreason` set IsActive = 0 where `Name` = 'Technical issue';
Update `abandonreason` set IsActive = 0 where `Name` = 'Not relevant to me';
Update `abandonreason` set IsActive = 0 where `Name` = 'Don''t understand';

SELECT @solvBrandId := Id FROM `Brand` WHERE `name` = 'Solv' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @solvBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @solvBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @solvBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @solvBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL;

SELECT @neatoBrandId := Id FROM `Brand` WHERE `name` = 'Neato' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @neatoBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @neatoBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @neatoBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @neatoBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL;

SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP SmartFriend' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @hpBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @hpBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @hpBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @hpBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL;

SELECT @otroBrandId := Id FROM `Brand` WHERE `name` = 'OTRO' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @otroBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @otroBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @otroBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @otroBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL; 

SELECT @lazadaBrandId := Id FROM `Brand` WHERE `name` = 'Lazada' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @lazadaBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @lazadaBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @lazadaBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @lazadaBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL;

SELECT @companyOneBrandId := Id FROM `Brand` WHERE `name` = 'CompanyOne' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @companyOneBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @companyOneBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @companyOneBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @companyOneBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL;

SELECT @companyTwoBrandId := Id FROM `Brand` WHERE `name` = 'CompanyTwo' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @companyTwoBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @companyTwoBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @companyTwoBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @companyTwoBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL;

SELECT @hpApacBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;

INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation)
SELECT * FROM (
	SELECT UUID(), 'Complaint', @hpApacBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Need more info', @hpApacBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Follow up', @hpApacBrandId as BrandId, 1, 0 UNION
	SELECT UUID(), 'Taking leave', @hpApacBrandId as BrandId, 1, 0
) itemsAbandonReason
WHERE BrandId IS NOT NULL;