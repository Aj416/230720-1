SET @brandId = UUID();

-- quiz
SET @quizId = UUID();
INSERT INTO `quiz` (`Id`, `Title`, `Description`, `AllowedMistakes`, `FailureMessage`, `SuccessMessage`)
VALUES
	(@quizId, 'Final Evaluation Assessment', 'You have successfully viewed the Just Eat 101 guidelines.', 0, 'To Solv for Just Eat you''re going to need pass the assessment. Hit the button below to brush up on your brand knowledge or try the quiz again.', 'You''re now fully authorised with Dell, so go ahead, pick up your first ticket and start earning right now.');

SET @questionId = UUID();
INSERT INTO `quizquestion` (`Id`, `QuizId`, `IsMultiChoice`, `Title`, `Order`, `Enabled`)
VALUES
	(@questionId, @quizId, 0, 'What brands are you solving for?', 0, 0);

INSERT INTO `quizquestionoption` (`Id`, `QuestionId`, `Correct`, `Text`, `Order`, `Enabled`)
VALUES
	(UUID(), @questionId, 0, 'HP', 0, 0),
	(UUID(), @questionId, 0, 'Samsung', 1, 0),
	(UUID(), @questionId, 1, 'Just Eat', 2, 0);

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
	`ProbingFormId`)
VALUES (
	@brandId, 'Just Eat', NULL, NULL,
	'#FF8000', -- Color
	'https://assets.solvnow.com/just-eat/JustEat.png', -- Thumbnail
	'https://assets.solvnow.com/just-eat/JustEat.png', -- Logo
	UTC_TIMESTAMP(), UTC_TIMESTAMP(),
	0, -- IsPractice
	0.0000, -- FeePercentage
	0.0, -- TicketPrice
	0.0000, -- VatRate
	NULL, NULL, NULL, -- paypal
	2, 1, 1, -- `PaymentRouteId`, `InvoicingEnabled`, `InvoicingDashboardEnabled`,
	'00000000-0000-0000-0000-000000000000', -- billing details
	1, -- AutomaticAuthorization
	'## CONGRATULATIONS! \n\n You have successfully viewed the Just Eat 101 guidelines. To represent Just Eat, you''ll now need to take a peer knowledge check.', -- InductionDoneMessage
	'Just Eat has put together this 101 guide to get you authorised to Solv for them.', -- InductionInstructions
	NULL, NULL, NULL, 0,-- `UnauthorizedMessage`, `AgreementContent`, `AgreementHeading`, `IsAgreementRequired`,
	@quizId, -- QuizId - TODO
	'SUPPORT SPECIALIST', -- AdvocateTitle
	NULL, -- CreateTicketInstructions
	'Chat with JustEat', -- CreateTicketHeader
	'We are here to help you', -- CreateTicketSubheader
	'https://assets.solvnow.com/just-eat/contract.txt', -- ContractUrl
	'https://assets.solvnow.com/just-eat/contract.txt', -- ContractInternalUrl
	0, 0, 0, -- `TagsEnabled`, `MultiTagEnabled`, `SubTagEnabled`
	4320, -- WaitMinutesToClose - ticket auto close after it is marked as solved - 3 days
	0, -- NpsEnabled
	0, 0, -- `TicketsExportEnabled`, `TicketsImportEnabled`,
	0, 0, -- `SuperSolversEnabled`, `PushBackToClientEnabled`
	0, NULL, -- `SposEnabled`, `SposEmail`
	NULL -- ProbingFormId
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

-- auto responses
INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
	(UUID(), @brandId, 1, 9, 'This support ticket has been open for 30 days. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.', 2592000, NULL, NULL),
	(UUID(), @brandId, 1, 14, 'Thanks for contacting support today and submitting a rating. Have a nice day.', NULL, NULL, NULL);

-- induction
SET @inductionSectionId = UUID();
INSERT INTO `inductionsection` (`Id`, `Name`, `Enabled`, `BrandId`, `Order`)
VALUES
	(@inductionSectionId, 'Product knowledge', 1, @brandId, 1);

INSERT INTO `inductionsectionitem` (`Id`, `Name`, `Source`, `Enabled`, `SectionId`, `Order`)
VALUES
	(UUID(), 'Solv flow', 'https://assets.solvnow.com/just-eat/solv-flow.html', 1, @inductionSectionId, 1);


-- api key
INSERT INTO `apikey` (`Id`, `BrandId`, `Key`, `UserId`, `CreatedDate`, `RevokedDate`, `ApplicationId`)
VALUES
	(UUID(), @brandId, 'just-eat-1', NULL, UTC_TIMESTAMP(), NULL, 'just-eat-1');
