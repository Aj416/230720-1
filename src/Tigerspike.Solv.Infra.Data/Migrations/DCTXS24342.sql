SET @brandId = UUID();

-- Get Billing Details
SELECT @billingDetailsId := Id FROM solv.billingdetails where `Name` = 'Concentrix' and Email is not null  LIMIT 1;

-- quiz
SET @quizId = UUID();
INSERT INTO `quiz` (`Id`, `Title`, `Description`, `AllowedMistakes`, `FailureMessage`, `SuccessMessage`)
VALUES
	(@quizId, 'Final Evaluation Assessment', 'You have successfully viewed the HP 101 guidelines. To represent Brand, you''ll now need to take a peer knowledge check. There are 15 questions but don''t worry, you still pass if you get three questions wrong.', 0, 'To Solv for HP Americas you’re going to need pass the quiz. The good news is you can try it multiple times. Brush up on your brand knowledge or try the quiz again', 'You''re now fully authorised with HP, so go ahead, pick up your first ticket and start earning right now.');

SET @questionId = UUID();
INSERT INTO `quizquestion` (`Id`, `QuizId`, `IsMultiChoice`, `Title`, `Order`, `Enabled`)
VALUES
	(@questionId, @quizId, 0, 'What brands are you solving for?', 0, 0);

INSERT INTO `quizquestionoption` (`Id`, `QuestionId`, `Correct`, `Text`, `Order`, `Enabled`)
VALUES
	(UUID(), @questionId, 1, 'HP', 0, 0),
	(UUID(), @questionId, 0, 'ByteDance', 1, 0),
	(UUID(), @questionId, 0, 'DELL', 2, 0);

-- general brand params
INSERT INTO `brand` (
	`Id`, `Name`, `Code`, `ShortCode`,
	`Color`,
	`Thumbnail`, `Logo`,
	`CreatedDate`, `ModifiedDate`,
	`IsPractice`,
	`FeePercentage`, `TicketPrice`, `VatRate`,
	`PaymentAccountId`, `BillingAgreementToken`, `BillingAgreementId`,
	`PaymentRouteId`, `InvoicingEnabled`, `InvoicingDashboardEnabled`,
	`BillingDetailsId`,
	`AutomaticAuthorization`,
	`InductionDoneMessage`, `InductionInstructions`,
	`UnauthorizedMessage`, `AgreementContent`, `AgreementHeading`, `IsAgreementRequired`,
	`QuizId`,
	`AdvocateTitle`, `CreateTicketInstructions`, `CreateTicketHeader`, `CreateTicketSubheader`,
	`ContractUrl`, `ContractInternalUrl`,
	`TagsEnabled`, `MultiTagEnabled`, `SubTagEnabled`,
	`WaitMinutesToClose`,
	`NpsEnabled`,
	`TicketsExportEnabled`, `TicketsImportEnabled`,
	`SuperSolversEnabled`, `PushBackToClientEnabled`,
	`SposEnabled`, `SposEmail`,
	`ProbingFormId`,`AdditionalFeedBackEnabled`,
	`CategoryEnabled`,`EndChatEnabled`)
VALUES (
	@brandId, 'HP AMS Print', 'HP Americas Print', 'HP AMS PRT',
	'#0097D5', -- Color
	'https://assets.solvnow.com/hp-ams-print/HP.png', -- Thumbnail
	'https://assets.solvnow.com/hp-ams-print/HP.png', -- Logo
	UTC_TIMESTAMP(), UTC_TIMESTAMP(),
	0, -- IsPractice
	0.3, -- FeePercentage
	2.80, -- TicketPrice
	0, -- VatRate
	NULL, NULL, NULL, -- paypal
	2, 1, 1, -- `PaymentRouteId`, `InvoicingEnabled`, `InvoicingDashboardEnabled`,
	@billingDetailsId, -- billing details
	1, -- AutomaticAuthorization
	'## CONGRATULATIONS! \n\n You have successfully viewed the HP AMS Print 101 guidelines. To represent HP AMS Print, you''ll now need to take a peer knowledge check.', -- InductionDoneMessage
	'HP AMS Print has put together this 101 guide to get you authorised to Solv for them. \n\n **NOTE: You must not copy, share or duplicate any of the contents of this section!** \n\n HP AMS Print is a global leader in home computing, you will be helping users of their products with out of warranty support, so it will be technical in nature on hardware, software and networking type questions. \n\n Support will be 24/7 but we think busiest from Monday to Friday, lighter over the weekend. We expect to see anything from 2000-3000 customers per month to start and grow. \n\n You will earn around $2.80 per chat ticket which will take between 3-10 mins of time to complete.', -- InductionInstructions
	NULL, NULL, NULL, 0,-- `UnauthorizedMessage`, `AgreementContent`, `AgreementHeading`, `IsAgreementRequired`,
	@quizId, -- QuizId - TODO
	'SUPPORT SPECIALIST', -- AdvocateTitle
	'By completing and submitting this form you consent to the use of your data in accordance with HP''s Privacy Statement.', -- CreateTicketInstructions
	'Initiate Chat Support', -- CreateTicketHeader
	'Before we chat, please let us know who you are and how we can help you', -- CreateTicketSubheader
	'https://assets.solvnow.com/hp-ams-print/contract.txt', -- ContractUrl
	'https://assets.solvnow.com/hp-ams-print/contract.txt', -- ContractInternalUrl
	1, 0, 1, -- `TagsEnabled`, `MultiTagEnabled`, `SubTagEnabled`
	4320, -- WaitMinutesToClose - ticket auto close after it is marked as solved - 3 days
	1, -- NpsEnabled
	1, 1, -- `TicketsExportEnabled`, `TicketsImportEnabled`,
	1, 0, -- `SuperSolversEnabled`, `PushBackToClientEnabled`
	1, 'spos-lead@brand.com', -- `SposEnabled`, `SposEmail`
	NULL, -- ProbingFormId
	1, -- AdditionalFeedBackEnabled
	1, -- CategoryEnabled
	1 -- EndChatEnabled
	);

 -- abandon reasons
INSERT INTO `abandonreason` (`Id`, `Name`, `BrandId`, `IsActive`, `IsForcedEscalation`, `IsBlockedAdvocate`, `IsAutoAbandoned`, `Action`)
VALUES
	(UUID(), 'Forced escalation', @brandId, 1, 1, 0, 0, NULL),
	(UUID(), 'Advocate was blocked', @brandId, 1, 0, 1, 0, NULL),
	(UUID(), 'Auto-abandoned', @brandId, 1, 0, 0, 1, NULL),
	(UUID(), 'Complaint', @brandId, 1, 0, 0, 0, NULL),
	(UUID(), 'Need more info', @brandId, 1, 0, 0, 0, NULL),
	(UUID(), 'Too difficult', @brandId, 1, 0, 0, 0, NULL),
	(UUID(), 'Follow up', @brandId, 1, 0, 0, 0, NULL),
	(UUID(), 'Taking leave', @brandId, 1, 0, 0, 0, NULL);

SELECT @CsatChatActionId := Id FROM solv.chataction where Type = 2;
SELECT @NpsChatActionId := Id FROM solv.chataction where Type = 3;
-- auto responses
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @brandId, 1, 0, 'Welcome to HP Support. My name is {{AdvocateFirstName}}. I will be your technical support today. Please give me a few moments while I review your issue. Note - For security reasons, DO NOT send credit card information via chat.\n\nI understand the situation is difficult. Please be assured that we will help you resolve the issue. Note: You will receive a message inviting you to let us know if the proposed solution has resolved the problem. Please provide us with your valuable input so we can better serve you.', NULL, NULL, NULL),
	(UUID(), @brandId, 1, 19, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @NpsChatActionId, 'Customer'),
    (UUID(), @brandId, 1, 20, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer'),
	(UUID(), @brandId, 1, 14, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, NULL);

-- api key
INSERT INTO `apikey` (`Id`, `BrandId`, `Key`, `UserId`, `CreatedDate`, `RevokedDate`, `ApplicationId`)
VALUES
	(UUID(), @brandId, 'hp-ams-print-1', NULL, UTC_TIMESTAMP(), NULL, 'hp-ams-print-1');

-- Tags
SET @hardware = UUID();
SET @software = UUID();
SET @upsell = UUID();
INSERT INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`, `Description`)
VALUES
	(@hardware, @brandId, 'hardware', 0, 1, NULL, NULL, NULL, NULL, 'For use when you have performed all troubleshooting and determined that it is a hardware failure. NOTE: This will escalate the case to a level 2 to set up an onsite engineer visit.'),
	(@software, @brandId, 'software', NULL, 1, NULL, NULL, NULL, NULL, 'This tag should be used when you have successfully resolved the issue yourself.  After you select this tag you will be asked to pick a sub tag to which best describes the chat outcome.'),
	(@upsell, @brandId, 'upsell-only', 0, 1, NULL, NULL, 0, 0, 'For use when the customer is ONLY wishing to make a purchase and required no other support. NOTE: this will transfer the chat directly to a sales agent! You will be asked to pick the sub tag which indicates what the customer is wishing to purchase AND complete a short description in a free text box e.g. customer wishes to extend their warranty.'),
	(UUID(), @brandId, 'CDAX: On-site raised', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'CDAX: Other', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'CDAX: CRT/TCO', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'CDAX: Status check', NULL, 1, 2, @hardware, 0, 0, NULL),
	(UUID(), @brandId, 'Referred to SVC', NULL, 0, NULL, @software, 0, 0, NULL),
	(UUID(), @brandId, 'Referred to website/self-help', NULL, 1, NULL, @software, 0, 0, 'the issue is out of warranty and you have referred the customer to SmartFriend or web help.'),
	(UUID(), @brandId, 'Troubleshooting fixed', NULL, 1, NULL, @software, 0, 0, 'you performed troubleshooting steps and resolved the customer''s issue.'),
	(UUID(), @brandId, 'General query', NULL, 1, NULL, @software, 0, 0, 'you resolved a simple query which did not require troubleshooting e.g. how do I check my warranty online?'),
	(UUID(), @brandId, 'Customer disconnected', NULL, 1, NULL, @software, 0, 0, 'customer went unresponsive during chat and is no longer responding. This tag should only be used AFTER you have followed the ''disconnected'' customer closing process from the script guide.'),
	(UUID(), @brandId, 'Referred to SVC or web', NULL, 0, NULL, @software, 0, 0, NULL),
	(UUID(), @brandId, 'Pending customer', NULL, 1, NULL, @software, 0, 0, 'you have given the customer steps to try which cannot be performed immediately or where the customer may get disconnected.'),
	(UUID(), @brandId, 'Warranty extension', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Care pack', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Accessory', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Notebook', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Desktop', NULL, 1, NULL, @upsell, 0, 0, NULL),
	(UUID(), @brandId, 'Printer', NULL, 1, NULL, @upsell, 0, 0, NULL);

-- category
INSERT IGNORE INTO `solv`.`category`
(`Id`, `BrandId`, `Name`, `Enabled`)
VALUES
(UUID(), @brandId, 'Battery issues', 1),
(UUID(), @brandId, 'Display Issues', 1),
(UUID(), @brandId, 'Performance and Overheating Issues', 1),
(UUID(), @brandId, 'Keyboard issues', 1),
(UUID(), @brandId, 'Boot Issues', 1),
(UUID(), @brandId, 'Touchpad Issues', 1),
(UUID(), @brandId, 'Product Upgradation', 1),
(UUID(), @brandId, 'Fan Issue', 1),
(UUID(), @brandId, 'MS Office Issue', 1),
(UUID(), @brandId, 'Hinges / Bazel / Periphery Issues', 1),
(UUID(), @brandId, 'Physical Damage', 1),
(UUID(), @brandId, 'No Power Issue', 1),
(UUID(), @brandId, 'Connectivity Issues', 1),
(UUID(), @brandId, 'General Product Queries / How to use', 1),
(UUID(), @brandId, 'Windows / Operating System Issues / Recovery Suit', 1),
(UUID(), @brandId, 'Audio Issues', 1),
(UUID(), @brandId, 'Webcam Issues', 1),
(UUID(), @brandId, 'Storage Issues', 1),
(UUID(), @brandId, 'Others', 1);

-- form setup/metadata

INSERT INTO `brandformfield` (`Id`, `Name`, `Title`, `TypeId`, `IsRequired`, `Validation`, `DefaultValue`, `Order`, `CreatedDate`, `ModifiedDate`, `BrandId`, `Options`, `IsKey`, `AccessLevel`,`Description`)
VALUES
	(UUID(), 'phoneNumber', 'Phone Number', 1, 1, 'required,phone', '', 2, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @brandId, NULL, 1, 3, NULL),
	(UUID(), 'serialNumber', 'Serial Number', 1, 1, 'required', '', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @brandId, NULL, 1, 3, '## Find your serial number\n\n### Notebook\n\nPress Fn+ESC keys\n\n### Desktop or All in One\n\nPress Ctrl+Alt+S keys'),
	(UUID(), 'productNumberOrModelName', 'Product Number or Model Name', 1, 1, 'required', '', 3, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @brandId, NULL, 0, 2, NULL),
	(UUID(), 'errorCode', 'Error code', 1, 0, '', '', 4, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @brandId, NULL, 0, 2,'## Do you have an error code?\n\nError codes are generated and presented on the HP device or display. Please include any error codes to provide additional information to the HP Support Agent.\n\nExample include:\n\n0FAF1D—000000-MFGJMA-C0D203\n\nHP Client Security Manager Error 1722\n\nDisplay driver stopped responding and has recovered\n\n');