SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

update solv.brandadvocateresponseconfig set Content = 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.\n\nI understand the situation is difficult. Please be assured that we will help you resolve the issue. Note: You will receive a message inviting you to let us know if the proposed solution has resolved the problem. Please provide us with your valuable input so we can better serve you.'
where BrandID = @amsId and Type in (0,2);

SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;

update solv.brandadvocateresponseconfig set Content = 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.\n\nI understand the situation is difficult. Please be assured that we will help you resolve the issue. Note: You will receive a message inviting you to let us know if the proposed solution has resolved the problem. Please provide us with your valuable input so we can better serve you.'
where BrandID = @indId and Type in (0,2);

SELECT @prntId := Id FROM `brand` WHERE `ShortCode` = 'HP AMS PRT' LIMIT 1;

update solv.brandadvocateresponseconfig set Content = 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.\n\nI understand the situation is difficult. Please be assured that we will help you resolve the issue. Note: You will receive a message inviting you to let us know if the proposed solution has resolved the problem. Please provide us with your valuable input so we can better serve you.'
where BrandID = @prntId and Type in (0,2);