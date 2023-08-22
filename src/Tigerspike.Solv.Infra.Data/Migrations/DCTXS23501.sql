SELECT @probingFormId := ProbingFormId FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `probingform` 
SET `Title` = 'Before we chat, please let us know who you are and how we can help you' 
WHERE `Id` = @probingFormId;

UPDATE `probingquestion` 
SET `Text` = 'Do you have an Open Case with a Case Number or are you looking for the status of your Repair?',
`Description` = '## Do you have an open case number?\n\nYour case number is a 10 digit number that begins with 5xxxxxxxxx' 
WHERE `ProbingFormId` = @probingFormId AND `Code` = 'CaseNumber';

UPDATE `probingquestion` 
SET `Text` = 'Are you experiencing a hardware related issue such as: Loud Noise, Screen Flickering, Unit not Powering on, Overheating?'
WHERE `ProbingFormId` = @probingFormId AND `Code` = 'DamagedDevice';   