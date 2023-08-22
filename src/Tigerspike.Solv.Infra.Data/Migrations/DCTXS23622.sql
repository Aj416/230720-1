-- Add Chat action for CSAT and NPS
insert into solv.chataction values (UUID(), 3, 1);
insert into solv.chataction values (UUID(), 2, 1);

-- Add Chat Action option for NPS
SELECT @NpsChatActionId := Id FROM solv.chataction where Type = 3;
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,0,0,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,1,1,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,2,2,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,3,3,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,4,4,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,5,5,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,6,6,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,7,7,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,8,8,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,9,9,0);
insert into solv.chatactionoption values(UUID(),@NpsChatActionId,10,10,0);

-- Add Chat Action option for Csat
SELECT @CsatChatActionId := Id FROM solv.chataction where Type = 2;
insert into solv.chatactionoption values(UUID(),@CsatChatActionId,1,1,0);
insert into solv.chatactionoption values(UUID(),@CsatChatActionId,2,2,0);
insert into solv.chatactionoption values(UUID(),@CsatChatActionId,3,3,0);
insert into solv.chatactionoption values(UUID(),@CsatChatActionId,4,4,0);
insert into solv.chatactionoption values(UUID(),@CsatChatActionId,5,5,0);

-- NPS and Csat configuration for HP Americas Brand
SELECT @hpAmsBrandId := Id FROM `Brand` WHERE `name` = 'HP Americas' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @hpAmsBrandId, 1, 19, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @NpsChatActionId, 'Customer'),
    (UUID(), @hpAmsBrandId, 1, 20, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- NPS and Csat configuration for HP India Brand
SELECT @hpIndiaBrandId := Id FROM `Brand` WHERE `name` = 'HP India' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @hpIndiaBrandId, 1, 19, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @NpsChatActionId, 'Customer'),
    (UUID(), @hpIndiaBrandId, 1, 20, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- NPS and Csat configuration for HP SmartFriend Brand
SELECT @hpSamrtFriendBrandId := Id FROM `Brand` WHERE `name` = 'HP SmartFriend' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @hpSamrtFriendBrandId, 1, 19, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @NpsChatActionId, 'Customer'),
    (UUID(), @hpSamrtFriendBrandId, 1, 20, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- NPS and Csat configuration for CompanyOne Brand
SELECT @companyOneBrandId := Id FROM `Brand` WHERE `name` = 'CompanyOne' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @companyOneBrandId, 1, 21, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @NpsChatActionId, 'Customer'),
    (UUID(), @companyOneBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for Solv Practice Brand
SELECT @solvPracticeBrandId := Id FROM `Brand` WHERE `name` = 'Solv Practice' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @solvPracticeBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for OTRO Brand
SELECT @otroBrandId := Id FROM `Brand` WHERE `name` = 'OTRO' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @otroBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for Neato Brand
SELECT @neatoBrandId := Id FROM `Brand` WHERE `name` = 'Neato' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @neatoBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for Lazada Brand
SELECT @lazadaBrandId := Id FROM `Brand` WHERE `name` = 'Lazada' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @lazadaBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for Solv Support Brand
SELECT @solvSupportBrandId := Id FROM `Brand` WHERE `name` = 'Solv Support' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @solvSupportBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for CompanyTwo Brand
SELECT @companyTwoBrandId := Id FROM `Brand` WHERE `name` = 'CompanyTwo' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @companyTwoBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for Samsung Italy Brand
SELECT @samsungItalyBrandId := Id FROM `Brand` WHERE `name` = 'Samsung Italy' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @samsungItalyBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');

-- Csat configuration for Dell Brand
SELECT @dellBrandId := Id FROM `Brand` WHERE `name` = 'Dell' limit 1;
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @dellBrandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');