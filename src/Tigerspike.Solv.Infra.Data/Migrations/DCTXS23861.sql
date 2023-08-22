SELECT @probingFormId := ProbingFormId FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;

UPDATE `probingquestion`
SET `Code` = 'CaseNumber'
WHERE `Code` = 'WA' AND `ProbingFormId` = @probingFormId;
