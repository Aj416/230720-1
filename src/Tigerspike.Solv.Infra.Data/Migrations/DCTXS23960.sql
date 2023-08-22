SELECT @brandId := Id FROM `brand` WHERE `IsPractice` = 1 LIMIT 1;

UPDATE `solv`.`brand`
SET
	`CategoryEnabled` = 1,
	`TagsEnabled` = 1,
	`SubTagEnabled` = 1,
	`SposEnabled` = 1
WHERE `Id` = @brandId;

-- category and spos description
UPDATE `solv`.`brand` 
SET
	`CategoryDescription` = 'This tag tells us what the customer''s primary issue was. For example if you determine a customer''s issue is related to their battery, you should use the battery tag. It''s really important you use the correct tags - this helps us improve onboarding content.',
	`SposDescription` = 'Select Yes if you have resolved the initial query but customer is ALSO wanting to make a purchase. If customer only wishes to make a purchase you should use the up-sell only tag. You will be asked to complete a short description in a free text box e.g. customer wishes to extend their warranty.'
WHERE `Id` = @brandId;

-- create categories
INSERT IGNORE INTO `category` (`Id`, `BrandId`, `Name`, `Enabled`, `Order`)
VALUES
	(UUID(), @brandId, 'Audio Issues', 1, 15),
	(UUID(), @brandId, 'Battery issues', 1, 0),
	(UUID(), @brandId, 'Boot Issues', 1, 4),
	(UUID(), @brandId, 'Connectivity Issues', 1, 12),
	(UUID(), @brandId, 'Display Issues', 1, 1),
	(UUID(), @brandId, 'Fan Issue', 1, 7),
	(UUID(), @brandId, 'General Product Queries / How to use', 1, 13),
	(UUID(), @brandId, 'Hinges / Bazel / Periphery Issues', 1, 9),
	(UUID(), @brandId, 'Keyboard issues', 1, 3),
	(UUID(), @brandId, 'MS Office Issue', 1, 8),
	(UUID(), @brandId, 'No Power Issue', 1, 11),
	(UUID(), @brandId, 'Others', 1, 18),
	(UUID(), @brandId, 'Performance and Overheating Issues', 1, 2),
	(UUID(), @brandId, 'Physical Damage', 1, 10),
	(UUID(), @brandId, 'Product Upgradation', 1, 6),
	(UUID(), @brandId, 'Storage Issues', 1, 17),
	(UUID(), @brandId, 'Touchpad Issues', 1, 5),
	(UUID(), @brandId, 'Webcam Issues', 1, 16),
	(UUID(), @brandId, 'Windows / Operating System Issues / Recovery Suit', 1, 14);

-- create tags
SET @hardware = UUID();
SET @software = UUID();
SET @upsell = UUID();
INSERT IGNORE INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`, `Description`)
VALUES
	(@hardware, @brandId, 'hardware', NULL, 1, NULL, NULL, 0, 0, 'For use when you have performed all troubleshooting and determined that it is a hardware failure. NOTE: This will escalate the case to a level 2 to set up an onsite engineer visit.'),
	(@software, @brandId, 'software', NULL, 1, NULL, NULL, 0, 0, 'This tag should be used when you have successfully resolved the issue yourself.  After you select this tag you will be asked to pick a sub tag to which best describes the chat outcome.'),
	(@upsell, @brandId, 'upsell-only', NULL, 1, NULL, NULL, 0, 0, 'For use when the customer is ONLY wishing to make a purchase and required no other support. NOTE: this will transfer the chat directly to a sales agent! You will be asked to pick the sub tag which indicates what the customer is wishing to purchase AND complete a short description in a free text box e.g. customer wishes to extend their warranty.'),
	(UUID(), @brandId, 'CDAX: On-site raised', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'CDAX: Other', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'CDAX: CRT/TCO', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'CDAX: Status check', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'Referred to SVC', NULL, 0, NULL, @software, 0, 0, NULL),
	(UUID(), @brandId, 'Referred to website/self-help', NULL, 1, NULL, @software, 0, 0, 'the issue is out of warranty and you have referred the customer to SmartFriend or web help.'),
	(UUID(), @brandId, 'Troubleshooting fixed', NULL, 1, NULL, @software, 0, 0, 'you performed troubleshooting steps and resolved the customer''s issue.'),
	(UUID(), @brandId, 'General query', NULL, 1, NULL, @software, 0, 0, 'you resolved a simple query which did not require troubleshooting e.g. how do I check my warranty online?'),
	(UUID(), @brandId, 'Customer disconnected', NULL, 1, NULL, @software, 0, 0, 'customer went unresponsive during chat and is no longer responding. This tag should only be used AFTER you have followed the ''disconnected'' customer closing process from the script guide.'),
	(UUID(), @brandId, 'Referred to SVC or web', NULL, 0, NULL, @software, 0, 0, NULL),
	(UUID(), @brandId, 'Pending customer', NULL, 1, NULL, @software, 0, 0, 'you have given the customer steps to try which cannot be performed immediately or where the customer may get disconnected.'),
	(UUID(), @brandId, 'Warranty extension', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Care pack', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Accessory', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Notebook', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Desktop', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Printer', NULL, 1, NULL, @upsell, 0, 0, NULL);