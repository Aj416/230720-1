SELECT @probingFormId := ProbingFormId FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

update solv.probingquestion set Description = '## Do you have an open case number?\n\nYour case number is a 10 digit number that begins with 5xxxxxxxxx'
where ProbingFormId = @probingFormId and `Order` = 1;