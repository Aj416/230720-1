SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`)
VALUES
	(UUID(), @hpBrandId, 1, 3, 'Thank you for your patience. Support is quite busy at present, we are prioritising your message and a support specialist will be with you shortly', 180);
