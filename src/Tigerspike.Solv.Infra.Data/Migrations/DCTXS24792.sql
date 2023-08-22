SELECT @amsId := Id FROM `Brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
SELECT @indId := Id FROM `Brand` WHERE `ShortCode` = 'IND' LIMIT 1;

UPDATE `brandadvocateresponseconfig`
SET `Content` = 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. \n\n <b>NOTE</b> - For security reasons, DO NOT send credit card information via chat. I understand the situation is difficult. Please be assured that we will help you resolve the issue. Note: You will receive a message inviting you to let us know if the proposed solution has resolved the problem. Please provide us with your valuable input so we can better serve you.'
WHERE `Type` IN (0,2) AND `BrandId` IN (@indId, @amsId); -- only hp brands