SET @brandId = UUID();

-- quiz
SET @quizId = UUID();
INSERT INTO `quiz` (`Id`, `Title`, `Description`, `AllowedMistakes`, `FailureMessage`, `SuccessMessage`)
VALUES (
	@quizId,
	'Final Evaluation Assessment',
	'You have successfully viewed the Solv Demo 101 guidelines. To represent Brand, you''ll now need to take a peer knowledge check. There are 15 questions but don''t worry, you still pass if you get three questions wrong.',
	1,
	'To Solv for Solv Demo you''re going to need pass the assessment.Hit the button below to brush up on your brand knowledge or try the quiz again.',
	'You''re now fully authorised with Solv Demo, so go ahead, pick up your first ticket and start earning right now.'
	);

SET @questionId = UUID();
INSERT INTO `quizquestion` (`Id`, `QuizId`, `IsMultiChoice`, `Title`, `Order`, `Enabled`)
VALUES
	(@questionId, @quizId, 0, 'What brands are you solving for?', 0, 0);

INSERT INTO `quizquestionoption` (`Id`, `QuestionId`, `Correct`, `Text`, `Order`, `Enabled`)
VALUES
	(UUID(), @questionId, 0, 'Incorrect answer 1', 0, 0),
	(UUID(), @questionId, 0, 'Fake brand', 1, 0),
	(UUID(), @questionId, 1, 'Solv Demo', 2, 0);

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
	`CategoryEnabled`, `ValidTransferEnabled`,
	`AdditionalFeedBackEnabled`,
	`ProbingFormId`)
VALUES (
	@brandId, 'Solv Demo', NULL, NULL,
	'#191847', -- Color
	'https://assets.solvnow.com/solv-demo/solv-demo-logo.png', -- Thumbnail
	'https://assets.solvnow.com/solv-demo/solv-demo-logo.png', -- Logo
	UTC_TIMESTAMP(), UTC_TIMESTAMP(),
	0, -- IsPractice
	0.3, -- FeePercentage
	0.77, -- TicketPrice
	0.2, -- VatRate
	NULL, NULL, NULL, -- paypal
	2, 1, 1, -- `PaymentRouteId`, `InvoicingEnabled`, `InvoicingDashboardEnabled`,
	'00000000-0000-0000-0000-000000000000', -- billing details
	1, -- AutomaticAuthorization
	'## CONGRATULATIONS! \n\n You have successfully viewed the Solv Demo 101 guidelines. To represent HP, you''ll now need to take a peer knowledge check.', -- InductionDoneMessage
	'Solv Demo has put together this 101 guide to get you authorised to Solv for them. \n\n **NOTE: You must not copy, share or duplicate any of the contents of this section!** \n\n Solv Demo is a global leader in home computing, you will be helping users of their products with out of warranty support, so it will be technical in nature on hardware, software and networking type questions. \n\n Support will be 24/7 but we think busiest from Monday to Friday, lighter over the weekend. We expect to see anything from 2000-3000 customers per month to start and grow. \n\n You will earn around $2.80 per chat ticket which will take between 3-10 mins of time to complete.', -- InductionInstructions
	NULL, NULL, NULL, 0,-- `UnauthorizedMessage`, `AgreementContent`, `AgreementHeading`, `IsAgreementRequired`,
	@quizId, -- QuizId - TODO
	'SUPPORT SPECIALIST', -- AdvocateTitle
	'By completing and submitting this form you consent to the use of your data in accordance with Solv Demo Privacy Statement.', -- CreateTicketInstructions
	'Initiate Chat Support', -- CreateTicketHeader
	'Before we chat, please let us know who you are and how we can help you', -- CreateTicketSubheader
	'https://assets.solvnow.com/solv-demo/contract.txt', -- ContractUrl
	'https://assets.solvnow.com/solv-demo/contract.txt', -- ContractInternalUrl
	1, 0, 1, -- `TagsEnabled`, `MultiTagEnabled`, `SubTagEnabled`
	4320, -- WaitMinutesToClose - ticket auto close after it is marked as solved - 3 days
	1, -- NpsEnabled
	1, 1, -- `TicketsExportEnabled`, `TicketsImportEnabled`,
	1, 1, -- `SuperSolversEnabled`, `PushBackToClientEnabled`
	1, 'solv-demo-spos-lead@mailinator.com', -- `SposEnabled`, `SposEmail`
	1, 1, -- CategoryEnabled, ValidTransferEnabled,
	1, -- AdditionalFeedBackEnabled
	NULL -- ProbingFormId
	);

-- customer form
INSERT INTO `brandformfield` (`Id`, `Name`, `Title`, `TypeId`, `IsRequired`, `Validation`, `DefaultValue`, `Order`, `CreatedDate`, `ModifiedDate`, `BrandId`, `Options`, `IsKey`, `AccessLevel`, `Description`)
VALUES
	(UUID(), 'phoneNumber', 'Phone Number', 1, 1, 'required,phone', '', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @brandId, NULL, 1, 3, NULL);


-- abandon reasons
INSERT INTO `abandonreason` (`Id`, `Name`, `BrandId`, `IsActive`, `IsForcedEscalation`, `IsBlockedAdvocate`, `IsAutoAbandoned`, `Action`)
VALUES
	(UUID(), 'Forced escalation', @brandId, 1, 1, 0, 0, NULL),
	(UUID(), 'Advocate was blocked', @brandId, 1, 0, 1, 0, NULL),
	(UUID(), 'Auto-abandoned', @brandId, 1, 0, 0, 1, NULL),
	(UUID(), 'Complaint', @brandId, 1, 0, 0, 0, NULL),
	(UUID(), 'Need more info', @brandId, 1, 0, 0, 0, NULL),
	(UUID(), 'Too difficult', @brandId, 1, 0, 0, 0, NULL),
	(UUID(), 'Follow up', @brandId, 1, 0, 0, 0, 0),
	(UUID(), 'Taking leave', @brandId, 1, 0, 0, 0, NULL);

SELECT @isTicketSolvedQuestionAction := Id FROM `chataction` WHERE `Type` = 1 LIMIT 1;
SELECT @csatAction := Id FROM `chataction` WHERE `Type` = 2 LIMIT 1;
SELECT @npsAction := Id FROM `chataction` WHERE `Type` = 3 LIMIT 1;
SELECT @feedbackAction := Id FROM `chataction` WHERE `Type` = 5 LIMIT 1;

-- auto responses
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`, `AuthorUserType`)
VALUES
	(UUID(), @brandId, 1, 0, 'Welcome to Solv Demo support. My name is {{AdvocateFirstName}}. I will be your support specialist today. Please give me a few moments while I review your issue. Please note, for security reasons, do NOT send credit card information via chat.', NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 2, 'Welcome to Solv Demo support. My name is {{AdvocateFirstName}}. I will be your support specialist today. Please give me a few moments while I review your issue. Please note, for security reasons, do NOT send credit card information via chat.', NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 3, 'Thank you for your patience. Support is quite busy at present, we are prioritising your message and a support specialist will be with you shortly', 60, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 4, 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.', NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 5, 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.\n    If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.', NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 6, NULL, 180, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 7, 'Welcome back {{CustomerFirstName}}. You already have a chat open with us so we''ve taken you straight to it.\n    Please help us improve.\n    Is your issues resolved?', NULL, @isTicketSolvedQuestionAction, 'Customer', NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 8, 'If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.', NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 10, 'Welcome back to the chat {{CustomerFirstName}}. If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.', NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 11, NULL, 180, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 12, 'Welcome back {{CustomerFirstName}}.\n    Please help us improve.\n    Is your issues resolved?', NULL, @isTicketSolvedQuestionAction, 'Customer', NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 13, 'If you don''t receive a reply from {{AdvocateFirstName}} within {{NextActionDelay}} we''ll find someone else who can help.', NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL),
	(UUID(), @brandId, 1, 16, NULL, NULL, NULL, NULL, 2, NULL, NULL, 100, NULL),
	(UUID(), @brandId, 1, 18, NULL, 180, NULL, NULL, NULL, NULL, NULL, 100, NULL),
	(UUID(), @brandId, 1, 19, 'Based on your recent support experience, how likely would you be to recommend {{BrandName}} to a friend, family member or a colleague?', NULL, @npsAction, 'Customer', NULL, NULL, NULL, 100, 1),
	(UUID(), @brandId, 1, 20, 'How would you rate this support?', NULL, @csatAction, 'Customer', NULL, NULL, NULL, 100, 1),
	(UUID(), @brandId, 1, 21, 'Help us by explaining your scores...', NULL, @feedbackAction, 'Customer', NULL, NULL, NULL, 100, 1),
	(UUID(), @brandId, 1, 22, 'Thanks for contacting support today and submitting a rating.', NULL, NULL, NULL, NULL, NULL, NULL, 100, 5);


-- induction
SET @inductionSectionId = UUID();
INSERT INTO `inductionsection` (`Id`, `Name`, `Enabled`, `BrandId`, `Order`)
VALUES
	(@inductionSectionId, 'Product knowledge', 1, @brandId, 1);

INSERT INTO `inductionsectionitem` (`Id`, `Name`, `Source`, `Enabled`, `SectionId`, `Order`)
VALUES
	(UUID(), 'Solv flow', 'https://assets.solvnow.com/solv-demo/solv-flow.html', 1, @inductionSectionId, 1);

-- issue categories
INSERT INTO `category` (`Id`, `BrandId`, `Name`, `Enabled`, `Order`)
VALUES
	(UUID(), @brandId, 'Audio Issues', 1, 15),
	(UUID(), @brandId, 'Battery issues', 1, 0),
	(UUID(), @brandId, 'Boot Issues', 1, 4),
	(UUID(), @brandId, 'Connectivity Issues', 1, 12),
	(UUID(), @brandId, 'Display Issues', 1, 1),
	(UUID(), @brandId, 'Fan Issue', 1, 7),
	(UUID(), @brandId, 'General Product Queries / How to use', 1, 13),
	(UUID(), @brandId, 'Hinges / Bazel / Periphery Issues', 1, 9),
	(UUID(), @brandId, 'Keyboard issues', 1, 3),
	(UUID(), @brandId, 'MS Office Issue', 1, 8),
	(UUID(), @brandId, 'No Power Issue', 1, 11),
	(UUID(), @brandId, 'Others', 1, 18),
	(UUID(), @brandId, 'Performance and Overheating Issues', 1, 2),
	(UUID(), @brandId, 'Physical Damage', 1, 10),
	(UUID(), @brandId, 'Product Upgradation', 1, 6),
	(UUID(), @brandId, 'Storage Issues', 1, 17),
	(UUID(), @brandId, 'Touchpad Issues', 1, 5),
	(UUID(), @brandId, 'Webcam Issues', 1, 16),
	(UUID(), @brandId, 'Windows / Operating System Issues / Recovery Suit', 1, 14);


-- tags
SET @hardware = UUID();
SET @software = UUID();
SET @upsell = UUID();
INSERT INTO `tag` (`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`, `Description`)
VALUES
	(@hardware, @brandId, 'hardware', 0, 1, NULL, NULL, NULL, NULL, 'For use when you have performed all troubleshooting and determined that it is a hardware failure. NOTE: This will escalate the case to a level 2 to set up an onsite engineer visit.'),
	(@software, @brandId, 'software', NULL, 1, NULL, NULL, NULL, NULL, 'This tag should be used when you have successfully resolved the issue yourself.  After you select this tag you will be asked to pick a sub tag to which best describes the chat outcome.'),
	(@upsell, @brandId, 'upsell-only', 0, 1, NULL, NULL, 0, 0, 'For use when the customer is ONLY wishing to make a purchase and required no other support. NOTE: this will transfer the chat directly to a sales agent! You will be asked to pick the sub tag which indicates what the customer is wishing to purchase AND complete a short description in a free text box e.g. customer wishes to extend their warranty.'),
	(UUID(), @brandId, 'CDAX: On-site raised', 0, 1, 2, @hardware, NULL, NULL, NULL),
	(UUID(), @brandId, 'CDAX: Other', 0, 1, 2, @hardware, NULL, NULL, NULL),
	(UUID(), @brandId, 'CDAX: CRT/TCO', 0, 1, 2, @hardware, NULL, NULL, NULL),
	(UUID(), @brandId, 'CDAX: Status check', 0, 1, 2, @hardware, NULL, NULL, NULL),
	(UUID(), @brandId, 'Referred to SVC', NULL, 0, NULL, @software, NULL, NULL, NULL),
	(UUID(), @brandId, 'Referred to website/self-help', NULL, 1, NULL, @software, NULL, NULL, 'the issue is out of warranty and you have referred the customer to SmartFriend or web help.'),
	(UUID(), @brandId, 'Troubleshooting fixed', NULL, 1, NULL, @software, NULL, NULL, 'you performed troubleshooting steps and resolved the customers issue.'),
	(UUID(), @brandId, 'General query', NULL, 1, NULL, @software, NULL, NULL, 'you resolved a simple query which did not require troubleshooting e.g. how do I check my warranty online?'),
	(UUID(), @brandId, 'Customer disconnected', NULL, 1, NULL, @software, NULL, NULL, 'customer went unresponsive during chat and is no longer responding. This tag should only be used AFTER you have followed the ''disconnected'' customer closing process from the script guide.'),
	(UUID(), @brandId, 'Referred to SVC or web', NULL, 0, NULL, @software, NULL, NULL, NULL),
	(UUID(), @brandId, 'Pending customer', NULL, 1, NULL, @software, NULL, NULL, 'you have given the customer steps to try which cannot be performed immediately or where the customer may get disconnected.'),
	(UUID(), @brandId, 'Warranty extension', 0, 1, NULL, @upsell, 0, NULL, NULL),
	(UUID(), @brandId, 'Care pack', 0, 1, NULL, @upsell, 0, NULL, NULL),
	(UUID(), @brandId, 'Accessory', 0, 1, NULL, @upsell, 0, NULL, NULL),
	(UUID(), @brandId, 'Notebook', 0, 1, NULL, @upsell, 0, NULL, NULL),
	(UUID(), @brandId, 'Desktop', 0, 1, NULL, @upsell, 0, NULL, NULL),
	(UUID(), @brandId, 'Printer', 0, 1, NULL, @upsell, 0, NULL, NULL);

-- probing form
SET @probingFormId = UUID();

INSERT INTO `probingform` VALUES (@probingFormId, 'Help us connect you to the right support team by answering a few questions');

SET @q1 = UUID();
SET @q2 = UUID();

INSERT INTO `probingquestion` (`Id`, `ProbingFormId`, `Text`, `Code`, `Description`, `Order`)
VALUES
	(@q1, @probingFormId, 'Do you have an Open Case Number', 'CaseNumber', 'This will cause escalation to WA', 1),
	(@q2, @probingFormId, 'Is your device physically damaged?', 'DamagedDevice', 'This will cause escalation to L2', 2);

INSERT INTO `probingquestionoption` (`Id`, `QuestionId`, `Text`, `Action`, `Order`, `Value`)
VALUES
	(UUID(), @q1, 'Yes', 1, 1, 'https://web.whatsapp.com'),
	(UUID(), @q1, 'No', NULL, 2, NULL),
	(UUID(), @q2, 'Yes', 0, 1, NULL),
	(UUID(), @q2, 'No', NULL, 2, NULL);

UPDATE `brand` SET ProbingFormId = @probingFormId WHERE Id = @brandId;


-- api key
INSERT INTO `apikey` (`Id`, `BrandId`, `Key`, `UserId`, `CreatedDate`, `RevokedDate`, `ApplicationId`)
VALUES
	(UUID(), @brandId, 'solv-demo-1', NULL, UTC_TIMESTAMP(), NULL, 'solv-demo-1');
