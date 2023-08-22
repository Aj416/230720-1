SELECT @hpAMSBrandId := Id FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @hpINDBrandId := Id FROM `Brand` WHERE `ShortCode` = 'IND' LIMIT 1;

UPDATE `solv`.`brand`
SET
`CategoryEnabled` = 1
WHERE `Id` IN (@hpAMSBrandId, @hpINDBrandId);

INSERT IGNORE INTO `solv`.`category`
(`Id`, `BrandId`, `Name`, `Enabled`)
VALUES
(UUID(), @hpAMSBrandId, 'Battery issues', 1),
(UUID(), @hpAMSBrandId, 'Display Issues', 1),
(UUID(), @hpAMSBrandId, 'Performance and Overheating Issues', 1),
(UUID(), @hpAMSBrandId, 'Keyboard issues', 1),
(UUID(), @hpAMSBrandId, 'Boot Issues', 1),
(UUID(), @hpAMSBrandId, 'Touchpad Issues', 1),
(UUID(), @hpAMSBrandId, 'Product Upgradation', 1),
(UUID(), @hpAMSBrandId, 'Fan Issue', 1),
(UUID(), @hpAMSBrandId, 'MS Office Issue', 1),
(UUID(), @hpAMSBrandId, 'Hinges / Bazel / Periphery Issues', 1),
(UUID(), @hpAMSBrandId, 'Physical Damage', 1),
(UUID(), @hpAMSBrandId, 'No Power Issue', 1),
(UUID(), @hpAMSBrandId, 'Connectivity Issues', 1),
(UUID(), @hpAMSBrandId, 'General Product Queries / How to use', 1),
(UUID(), @hpAMSBrandId, 'Windows / Operating System Issues / Recovery Suit', 1),
(UUID(), @hpAMSBrandId, 'Audio Issues', 1),
(UUID(), @hpAMSBrandId, 'Webcam Issues', 1),
(UUID(), @hpAMSBrandId, 'Storage Issues', 1),
(UUID(), @hpAMSBrandId, 'Others', 1);

INSERT IGNORE INTO `solv`.`category`
(`Id`, `BrandId`, `Name`, `Enabled`)
VALUES
(UUID(), @hpINDBrandId, 'Battery issues', 1),
(UUID(), @hpINDBrandId, 'Display Issues', 1),
(UUID(), @hpINDBrandId, 'Performance and Overheating Issues', 1),
(UUID(), @hpINDBrandId, 'Keyboard issues', 1),
(UUID(), @hpINDBrandId, 'Boot Issues', 1),
(UUID(), @hpINDBrandId, 'Touchpad Issues', 1),
(UUID(), @hpINDBrandId, 'Product Upgradation', 1),
(UUID(), @hpINDBrandId, 'Fan Issue', 1),
(UUID(), @hpINDBrandId, 'MS Office Issue', 1),
(UUID(), @hpINDBrandId, 'Hinges / Bazel / Periphery Issues', 1),
(UUID(), @hpINDBrandId, 'Physical Damage', 1),
(UUID(), @hpINDBrandId, 'No Power Issue', 1),
(UUID(), @hpINDBrandId, 'Connectivity Issues', 1),
(UUID(), @hpINDBrandId, 'General Product Queries / How to use', 1),
(UUID(), @hpINDBrandId, 'Windows / Operating System Issues / Recovery Suit', 1),
(UUID(), @hpINDBrandId, 'Audio Issues', 1),
(UUID(), @hpINDBrandId, 'Webcam Issues', 1),
(UUID(), @hpINDBrandId, 'Storage Issues', 1),
(UUID(), @hpINDBrandId, 'Others', 1);