SELECT @brandId := Id FROM solv.brand where `ShortCode` = 'WN' LIMIT 1;

INSERT INTO solv.brandmetadataroutingconfig
VALUES (uuid(), @brandId, 'urgencyLevel', 'urgent', 2);

INSERT INTO solv.brandmetadataaccess
VALUES (uuid(), @brandId, 'urgencyLevel', 2);