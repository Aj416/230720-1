
SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

SELECT @AmsSoftwareTagId := id FROM solv.tag where BrandId = @amsId and Name = 'Software';

INSERT INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`, `Description`,`L1PostClosureDisable`,`L2PostClosureDisable`)
VALUES
	(UUID(), @amsId, 'Unsupported Geo', 0, 1, 2, @AmsSoftwareTagId, NULL, NULL, 'Select Unsupported Geo when the customer has contacted us regarding a product that was purchased in a country not supported by this channel. For example, a customer who purchased their product in UK has reached out for support on HP AMS brand.',0,0);

-- Hp India
SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;

SELECT @IndSoftwareTagId := id FROM solv.tag where BrandId = @indId and Name = 'Software';

INSERT INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`, `Description`,`L1PostClosureDisable`,`L2PostClosureDisable`)
VALUES
	(UUID(), @indId, 'Unsupported Geo', 0, 1, 2, @IndSoftwareTagId, NULL, NULL, 'Select Unsupported Geo when the customer has contacted us regarding a product that was purchased in a country not supported by this channel. For example, a customer who purchased their product in UK has reached out for support on HP AMS brand.',0,0);