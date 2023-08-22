SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;

DELETE FROM inductionsection WHERE BrandId = @hpBrandId AND `name`='Product knowledge';