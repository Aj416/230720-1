SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;

INSERT INTO tag VALUES(UUID(), @hpBrandId, 'hardware', 1, 0);
INSERT INTO tag VALUES(UUID(), @hpBrandId, 'software', 1, NULL);