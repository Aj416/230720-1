SELECT @hpAmsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

update solv.brand set DiagnosisDescription = 'Select Correct if the level 1 had correctly diagnosed a hardware failure. If there was no hardware failure OR the level 1 did not perform troubleshooting, you should select Incorrect.' where Id = @hpAmsId;

SELECT @hpIndId := Id FROM `Brand` WHERE `ShortCode` = 'IND' LIMIT 1;

update solv.brand set DiagnosisDescription = 'Select Correct if the level 1 had correctly diagnosed a hardware failure. If there was no hardware failure OR the level 1 did not perform troubleshooting, you should select Incorrect.' where Id = @hpIndId;