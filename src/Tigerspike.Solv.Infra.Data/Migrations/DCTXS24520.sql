SELECT @brandId := Id FROM solv.brand where `Name` = 'HP AMS Print';

update solv.brandadvocateresponseconfig  set DelayInSeconds = 600 where Type = 9
and brandId = @brandId;