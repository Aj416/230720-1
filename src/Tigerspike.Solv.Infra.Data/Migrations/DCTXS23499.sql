SELECT @hpAmsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `brand` SET CreateTicketHeader = CONCAT('Chat with ', Name);

UPDATE `brand` SET
	CreateTicketHeader = 'Initiate Chat Support',
	CreateTicketSubheader = 'Before we chat, please let us know who you are and how we can help you',
	CreateTicketInstructions = 'By completing and submitting this form you consent to the use of your data in accordance with HP’s Privacy Statement. [Learn more about HP’s privacy policy](https://www8.hp.com/us/en/privacy/privacy.html)'
WHERE `Id` = @hpAmsId;

INSERT IGNORE INTO `brandformfield` (`Id`, `Name`, `Title`, `Description`, `TypeId`, `IsRequired`, `Validation`, `DefaultValue`, `Order`, `CreatedDate`, `ModifiedDate`, `BrandId`, `Options`, `IsKey`, `AccessLevel`)
VALUES
	(UUID(), 'errorCode', 'Error code', '## Do you have an error code?\n\nError codes are generated and presented on the HP device or display. Please include any error codes to provide additional information to the HP Support Agent.\n\nExample include:\n\n0FAF1D—000000-MFGJMA-C0D203\n\nHP Client Security Manager Error 1722\n\nDisplay driver stopped responding and has recovered\n\n',  1, 0, '', '', 4, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @hpAmsId, NULL, 0, 2);

UPDATE `brandformfield` SET
	`Description` = '## Find your serial number\n\n### Notebook\n\nPress Fn+ESC keys\n\n### Desktop or All in One \n\nPress Ctrl+Alt+S keys\n\n'
WHERE `Name`= 'serialNumber';

UPDATE `brandformfield` SET `Order` = 1 WHERE `Name`= 'phoneNumber';
UPDATE `brandformfield` SET `Order` = 2 WHERE `Name`= 'serialNumber';