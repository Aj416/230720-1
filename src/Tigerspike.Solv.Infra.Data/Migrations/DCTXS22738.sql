SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' LIMIT 1;

UPDATE `Brand` SET SposEnabled = 1 WHERE Id = @hpBrandId;

INSERT IGNORE INTO `Tag` VALUES(UUID(), @hpBrandId, 'upsell-only', 1, NULL);