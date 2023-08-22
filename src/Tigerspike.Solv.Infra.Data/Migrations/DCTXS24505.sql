SELECT @brandId := Id FROM solv.brand where `Name` = 'HP AMS Print';
update solv.brandadvocateresponseconfig set RelevantTo = 'Customer' where BrandId = @brandId and `Type` = 22;