INSERT INTO solv.brandadvocateresponseconfig(Id, BrandId, IsActive, Type, Content, Priority)
VALUES(UUID(), NULL, 1, 23, 'A support specialist will be with you shortly. Thanks for your patience.', 0);

SELECT @brandId := Id FROM solv.brand where `ShortCode` = 'WN' LIMIT 1;

INSERT INTO solv.brandadvocateresponseconfig(Id, BrandId, IsActive, Type, Content, Priority)
VALUES(UUID(), @brandId, 1, 23, 'A licensed professional therapist will be with you as soon as possible. \nThanks for your patience.',0);
