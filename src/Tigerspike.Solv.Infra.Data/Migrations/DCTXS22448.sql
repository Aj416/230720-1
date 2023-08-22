-- -------------------------------------------
-- setup prerequiesites
-- -------------------------------------------
INSERT IGNORE INTO `paymentroute` (`Id`, `Name`)
VALUES
	(1, 'Route1'),
	(2, 'Route2');

INSERT IGNORE INTO `billingdetails` (`Id`, `Name`, `Email`, `VatNumber`, `CompanyNumber`, `Address`, `IsPlatformOwner`, `CreatedDate`)
VALUES
	('00000000-0000-0000-0000-000000000000', 'Unspecified', NULL, NULL, NULL, NULL, 0, UTC_TIMESTAMP());

INSERT IGNORE INTO `brandformfieldtype` (`Id`, `Name`)
VALUES
	(1, 'input'),
	(2, 'textarea'),
	(3, 'select');


-- -------------------------------------------
-- migrate Solv -> Solv Practice brand
-- -------------------------------------------
UPDATE `brand` SET
	`Name` = 'Solv Practice',
	`Thumbnail` = 'https://assets.solvnow.com/solv-practice/Solv+Practice%402x.png',
	`Logo` = 'https://assets.solvnow.com/solv-practice/Solv+Practice%403x.png',
	`Color` = '#FBB218'
WHERE `Name` = 'Solv';

-- -------------------------------------------
-- create new Solv Support brand
-- -------------------------------------------
SET @solvSupportId := UUID();

INSERT INTO `brand`
	(
		`Id`,
		`Name`,
		`Thumbnail`,
		`Logo`,
		`CreatedDate`, `ModifiedDate`,
		`IsPractice`, `FeePercentage`, `TicketPrice`, `PaymentAccountId`, `BillingAgreementToken`, `BillingAgreementId`, `BillingDetailsId`, `VatRate`, `AutomaticAuthorization`, `InductionDoneMessage`, `InductionInstructions`, `UnauthorizedMessage`, `AgreementContent`, `AgreementHeading`, `IsAgreementRequired`, `QuizId`, `PaymentRouteId`, `InvoicingEnabled`, `InvoicingDashboardEnabled`, `AdvocateTitle`, `CreateTicketInstructions`, `CreateTicketSubheader`,
		`ContractUrl`, `ContractInternalUrl`,
		`TagsEnabled`, `WaitMinutesToClose`, `NpsEnabled`, `TicketsExportEnabled`, `SuperSolversEnabled`, `Color`, `PushBackToClientEnabled`)
VALUES
	(
		@solvSupportId,
		'Solv Support',
		'https://assets.solvnow.com/solv-support/thumbnail.png',
		'https://assets.solvnow.com/solv-support/logo.png',
		UTC_TIMESTAMP(), UTC_TIMESTAMP(),
		0, 0, 0, NULL, NULL, NULL, '00000000-0000-0000-0000-000000000000', NULL, 1, '', NULL, NULL, NULL, NULL, 0, NULL, 2, 0, 0, NULL, NULL, NULL,
		'https://assets.solvnow.com/solv-support/contract.txt', 'https://assets.solvnow.com/solv-support/contract.txt',
		0, 1440, 0, 0, 0, '#EA007C', 0);

INSERT INTO `abandonreason` (`Id`, `Name`, `BrandId`, `IsActive`, `IsForcedEscalation`, `IsBlockedAdvocate`, `IsAutoAbandoned`)
VALUES
	(UUID(), 'Complaint', @solvSupportId, 1, 0, 0, 0),
	(UUID(), 'Need more info', @solvSupportId, 1, 0, 0, 0),
	(UUID(), 'Too difficult', @solvSupportId, 1, 0, 0, 0),
	(UUID(), 'Follow up', @solvSupportId, 1, 0, 0, 0),
	(UUID(), 'Taking leave', @solvSupportId, 1, 0, 0, 0),
	(UUID(), 'Advocate was blocked', @solvSupportId, 1, 0, 1, 0),
	(UUID(), 'Forced escalation', @solvSupportId, 1, 1, 0, 0),
	(UUID(), 'Auto-abandoned', @solvSupportId, 1, 0, 0, 1);


INSERT INTO `brandformfield`
	(`Id`, `Name`, `Title`, `TypeId`, `IsRequired`, `Validation`, `DefaultValue`, `Order`, `CreatedDate`, `ModifiedDate`, `BrandId`, `Options`, `IsKey`)
VALUES
	(UUID(), 'problemCategory', 'Problem category', 3, 1, 'required', '', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @solvSupportId, 'Login issue,Technical issue,Payments,General feedback,Other', 0);

INSERT INTO `apikey`
	(`Id`, `BrandId`, `Key`, `UserId`, `CreatedDate`, `RevokedDate`, `ApplicationId`)
VALUES
	(UUID(), @solvSupportId, 'solv-support-1', NULL, UTC_TIMESTAMP(), NULL, 'solv-support-1');

