SELECT @hpAmsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `brand` SET
	CreateTicketInstructions = 'By completing and submitting this form you consent to the use of your data in accordance with HP''s Privacy Statement. [Learn more about HP''s privacy policy](https://www8.hp.com/us/en/privacy/privacy.html)'
WHERE `Id` = @hpAmsId;

UPDATE `brandformfield` SET
	`Description` = '## Do you have an error code?\n\nError codes are generated and presented on the HP device or display. Please include any error codes to provide additional information to the HP Support Agent.\n\nExample include:\n\n0FAF1D-000000-MFGJMA-C0D203\n\nHP Client Security Manager Error 1722\n\nDisplay driver stopped responding and has recovered\n'
WHERE `Name`= 'errorCode' AND BrandId = @hpAmsId;