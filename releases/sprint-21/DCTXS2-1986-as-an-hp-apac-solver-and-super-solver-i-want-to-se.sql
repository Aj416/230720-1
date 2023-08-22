SELECT @hpBrandId := Id
FROM `Brand`
WHERE `name` = 'HP'
limit 1;

-- insert default auto-respones for the HP brand
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `Type`, `Content`)
VALUES (
		UUID(),
		@hpBrandId,
		0,
		'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.'
	),
	(
		UUID(),
		@hpBrandId,
		2,
		'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.'
	);