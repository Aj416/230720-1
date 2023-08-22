SELECT @probingFormId := ProbingFormId FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;

UPDATE `probingquestion`
SET `Code` = 'WA'
WHERE `Code` = 'CaseNumber' AND `ProbingFormId` = @probingFormId;
