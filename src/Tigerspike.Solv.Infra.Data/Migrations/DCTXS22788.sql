SELECT @neatoBrandId := Id FROM `Brand` WHERE `name` = 'Neato' LIMIT 1;
SELECT @hpSmartFriendBrandId := Id FROM `Brand` WHERE `name` = 'HP SmartFriend' LIMIT 1;
SELECT @otroBrandId := Id FROM `Brand` WHERE `name` = 'Otro' LIMIT 1;
SELECT @lazadaBrandId := Id FROM `Brand` WHERE `name` = 'Lazada' LIMIT 1;
SELECT @solvSupportBrandId := Id FROM `Brand` WHERE `name` = 'Solv Support' LIMIT 1;

SELECT @message := 'This support ticket has been open for 30 days. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.';

INSERT IGNORE INTO `brandadvocateresponseconfig` VALUES(UUID(),@neatoBrandId,1,9,@message, 2592000,null,null);
INSERT IGNORE INTO `brandadvocateresponseconfig` VALUES(UUID(),@hpSmartFriendBrandId,1,9,@message, 2592000,null,null);
INSERT IGNORE INTO `brandadvocateresponseconfig` VALUES(UUID(),@otroBrandId,1,9,@message, 2592000,null,null);
INSERT IGNORE INTO `brandadvocateresponseconfig` VALUES(UUID(),@lazadaBrandId,1,9,@message, 2592000,null,null);
INSERT IGNORE INTO `brandadvocateresponseconfig` VALUES(UUID(),@solvSupportBrandId,1,9,@message, 2592000,null,null);