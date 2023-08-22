SELECT @probingFormId := ProbingFormId FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `probingform` 
SET `Title` = 'Help us connect you to the right support team by answering a few questions' 
WHERE `Id` = @probingFormId;