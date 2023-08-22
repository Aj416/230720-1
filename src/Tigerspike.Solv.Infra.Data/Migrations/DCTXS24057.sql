SET @brandId = UUID();

-- quiz
SET @quizId = UUID();
INSERT INTO `quiz` (`Id`, `Title`, `Description`, `AllowedMistakes`, `FailureMessage`, `SuccessMessage`)
VALUES
	(@quizId, 'Final Evaluation Assessment', 'You have successfully viewed the Brand 101 guidelines. To represent Brand, you''ll now need to take a peer knowledge check. There are 15 questions but don''t worry, you still pass if you get three questions wrong.', 0, 'To Solv for Brand you''re going to need pass the assessment.Hit the button below to brush up on your brand knowledge or try the quiz again.', 'You''re now fully authorised with Brand, so go ahead, pick up your first ticket and start earning right now.');

SET @questionId = UUID();
INSERT INTO `quizquestion` (`Id`, `QuizId`, `IsMultiChoice`, `Title`, `Order`, `Enabled`)
VALUES
	(@questionId, @quizId, 0, 'What brands are you solving for?', 0, 0);

INSERT INTO `quizquestionoption` (`Id`, `QuestionId`, `Correct`, `Text`, `Order`, `Enabled`)
VALUES
	(UUID(), @questionId, 0, 'HP', 0, 0),
	(UUID(), @questionId, 1, 'Samsung', 1, 0),
	(UUID(), @questionId, 0, 'ByteDance', 2, 0);

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
	`ProbingFormId`,`AdditionalFeedBackEnabled`)
VALUES (
	@brandId, 'Samsung', NULL, NULL,
	'#2655A6', -- Color
	'https://assets.solvnow.com/samsung-australia/Samsung.png', -- Thumbnail
	'https://assets.solvnow.com/samsung-australia/Samsung.png', -- Logo
	UTC_TIMESTAMP(), UTC_TIMESTAMP(),
	0, -- IsPractice
	0.3, -- FeePercentage
	0.0, -- TicketPrice
	20, -- VatRate
	NULL, NULL, NULL, -- paypal
	1, 0, 0, -- `PaymentRouteId`, `InvoicingEnabled`, `InvoicingDashboardEnabled`,
	'00000000-0000-0000-0000-000000000000', -- billing details
	1, -- AutomaticAuthorization
	'## CONGRATULATIONS! \n\n You have successfully completed your Samsung Australia induction. You can refer to the guidelines below at anytime.', -- InductionDoneMessage
	'Samsung has put together this 101 guide to get you authorised to Solv for them.', -- InductionInstructions
	NULL, NULL, NULL, 0,-- `UnauthorizedMessage`, `AgreementContent`, `AgreementHeading`, `IsAgreementRequired`,
	@quizId, -- QuizId - TODO
	'SUPPORT SPECIALIST', -- AdvocateTitle
	'By completing and submitting this form you consent to the use of your data in accordance with Otro''s Privacy Statement.', -- CreateTicketInstructions
	'Initiate Chat Support', -- CreateTicketHeader
	'Before we chat, please let us know who you are and how we can help you', -- CreateTicketSubheader
	'', -- ContractUrl
	'', -- ContractInternalUrl
	0, 0, 0, -- `TagsEnabled`, `MultiTagEnabled`, `SubTagEnabled`
	4320, -- WaitMinutesToClose - ticket auto close after it is marked as solved - 3 days
	0, -- NpsEnabled
	1, 1, -- `TicketsExportEnabled`, `TicketsImportEnabled`,
	0, 0, -- `SuperSolversEnabled`, `PushBackToClientEnabled`
	0, NULL, -- `SposEnabled`, `SposEmail`
	NULL, -- ProbingFormId
	1 --AdditionalFeedBackEnabled
	);

-- Billing details
insert into solv.billingdetails
(Id,`Name`,Email,VatNumber,CompanyNumber,Address,IsPlatformOwner,CreatedDate
)
values
(@brandId,'Concentrix',null,'GB219627101','NI037606',
'Concentrix Europe Limited
Mayfield
49 East Bridge Street
Belfast
BT1 3NR
United Kingdom',
1,UTC_TIMESTAMP()
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
-- auto responses
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @brandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer'),
	(UUID(), @brandId, 1, 14, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, NULL);

-- api key
INSERT INTO `apikey` (`Id`, `BrandId`, `Key`, `UserId`, `CreatedDate`, `RevokedDate`, `ApplicationId`)
VALUES
	(UUID(), @brandId, 'samsung-1', NULL, UTC_TIMESTAMP(), NULL, 'samsung-1');
